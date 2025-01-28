﻿using AsmResolver.DotNet;
using SimEi.Obfuscator.Renaming.Permission;
using SimEi.Obfuscator.Renaming.Reference;
using SimEi.Obfuscator.Renaming.RenameLog;
using SimEi.Obfuscator.Renaming.SigGraph;

namespace SimEi.Obfuscator.Renaming
{
    internal class RenamingPipeline
    {
        private readonly IMetadataResolver _metadataResolver;
        private readonly IRenamingPermissions _externalPermissions;

        public RenamingPipeline(IMetadataResolver metadataResolver, IRenamingPermissions externalPermissions)
        {
            _metadataResolver = metadataResolver;
            _externalPermissions = externalPermissions;
        }



        public void Rename(IEnumerable<ModuleDefinition> modules, IRenamingLogger logger)
        {
            Logger.Log("Start renaming...");
            Logger.Log("Renamed modules:");
            foreach (var module in modules)
                Logger.Log($"  - {module.Name}", false);

            Logger.Log("Finding references...");
            var msg = new MethodSigGraph();
            var refTracker = new ReferenceTracker(_metadataResolver, msg);
            var obfAttrPerm = new ObfuscationAttributePermissions();
            foreach (var module in modules)
            {
                Visit(module, refTracker);
                Visit(module, obfAttrPerm);
            }
            Logger.Log($"Found {refTracker.ReferenceCount} references.");

            Logger.Log("Executing renaming...");
            var runtimePerm = new RuntimeExcludedPermissions();
            var compPerm = new CompositeRenamingPermissions(obfAttrPerm, runtimePerm, _externalPermissions);
            RenameVirtualMethods(msg, modules, compPerm);

            var namingContext = new NamingContext();
            var excludeSigGraphPerm = new ConcreteExcludedPermissions(msg.Nodes);
            var finalPerm = new CompositeRenamingPermissions(compPerm, excludeSigGraphPerm);
            var renamer = new Renamer(namingContext, finalPerm, logger);
            foreach (var module in modules)
                Visit(module, renamer);

            Logger.Log("Fixing references...");
            refTracker.FixTrackedReferences();

            Logger.Log("Done!");
        }


        private void Visit(ModuleDefinition module, IModuleVisitor visitor)
        {
            foreach (var type in module.TopLevelTypes)
                Visit(type, visitor);
        }

        private void Visit(TypeDefinition type, IModuleVisitor visitor)
        {
            visitor.VisitType(type);

            foreach (var field in type.Fields)
                visitor.VisitField(field);
            foreach (var prop in type.Properties)
                visitor.VisitProp(prop);
            foreach (var evt in type.Events)
                visitor.VisitEvent(evt);

            foreach (var method in type.Methods)
            {
                visitor.VisitMethod(method);

                if (method.CilMethodBody == null)
                    continue;

                foreach (var local in method.CilMethodBody.LocalVariables)
                    visitor.VisitLocal(local);

                foreach (var inst in method.CilMethodBody.Instructions)
                    visitor.VisitInstruction(inst);
                foreach (var excHandler in method.CilMethodBody.ExceptionHandlers)
                    visitor.VisitExceptionHandler(excHandler);
            }

            foreach (var st in type.NestedTypes)
                Visit(st, visitor);
        }


        private void RenameVirtualMethods(MethodSigGraph virtMethodGraph, IEnumerable<ModuleDefinition> modules,
            IRenamingPermissions permissions)
        {
            var depMethodsNamingContext = new NamingContext("__");
            var modulesSet = modules.ToHashSet();
            foreach (var comp in virtMethodGraph.GetComponents())
            {
                if (!comp.AreAllMethodsControlled(_metadataResolver, modulesSet)
                    || comp.Nodes.Any(n => !permissions.CanRename(n)))
                    continue;

                // TODO: find type components from method components and reset naming context for each.
                string name = depMethodsNamingContext.GetNextName(null, RenamedElementType.Method);
                foreach (var method in comp.Nodes)
                {
                    method.Name = name;
                    foreach (var param in method.ParameterDefinitions)
                    {
                        if (permissions.CanRename(param))
                            param.Name = null;
                    }
                    foreach (var gparam in method.GenericParameters)
                        gparam.Name = depMethodsNamingContext.GetNextName(method.DeclaringType, RenamedElementType.GenericParameter);
                }
            }
        }
    }
}
