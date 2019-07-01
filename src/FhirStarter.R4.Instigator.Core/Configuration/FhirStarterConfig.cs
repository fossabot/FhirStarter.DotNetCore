﻿using System;
using System.Collections.Generic;
using System.Reflection;
using FhirStarter.R4.Detonator.Core.Interface;
using FhirStarter.R4.Instigator.Core.Helper;
using FhirStarter.R4.Instigator.Core.Model;
using FhirStarter.R4.Instigator.Core.Validation;
using Hl7.Fhir.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;

namespace FhirStarter.R4.Instigator.Core.Configuration
{
    public static class FhirStarterConfig
    {
        public static void SetupFhir(IServiceCollection services, IConfigurationRoot fhirStarterSettings, CompatibilityVersion dotNetCoreVersion)
        {
            AddFhirStarterSettings(services, fhirStarterSettings);
            RegisterServices(services, fhirStarterSettings);
        }

        /// <summary>
        /// Used in AddApplicationPart after .AddMvc
        /// </summary>
        /// <returns></returns>
        public static Assembly GetDetonatorAssembly() 
        {
            return GetReferencedAssembly("FhirStarter.R4.Detonator.Core");
        }

        /// <summary>
        /// Used in AddApplicationPart after .AddMvc
        ///
        /// Contains the FhirController, must add AddControllersAsServices after the AddApplicationPart
        /// ex:
        /// .AddApplicationPart(instigator).AddApplicationPart(detonator).AddControllersAsServices()
        /// </summary>
        /// <returns></returns>
        public static Assembly GetInstigatorAssembly()
        {
            return GetReferencedAssembly("FhirStarter.R4.Instigator.Core");
        }

        //https://github.com/dotnet/corefx/issues/11639
        private static Assembly GetReferencedAssembly(string assemblyName)
        {
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (library.Name.ToLower().Equals(assemblyName.ToLower()))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    return assembly;
                }
            }
            throw new ArgumentException($"Could not find {assemblyName} in DependencyContext. Is the assembly added to the main project?");
            
        }

        #region Assembly

        private static void AddFhirStarterSettings(IServiceCollection services, IConfigurationRoot fhirStarterSettings)
        {
            services.Add(new ServiceDescriptor(typeof(IConfigurationRoot),fhirStarterSettings));
        }

        private static void RegisterServices(IServiceCollection services, IConfigurationRoot fhirStarterSettings)
        {
            var fhirService = typeof(IFhirService);

            var serviceTypes = new List<TypeInitializer>
            {
                new TypeInitializer(true, fhirService, nameof(IFhirService))
            };

            var fhirServiceAssemblies = FhirStarterSettingsHelper.GetFhirServiceAssemblies(fhirStarterSettings);

            foreach (var asm in fhirServiceAssemblies)
            {
                var types = asm.GetTypes();
                foreach (var classType in types)
                {
                    BindIFhirServices(services, serviceTypes, classType);
                }
            }

            BindProfileValidator(services);
        }

        private static void BindProfileValidator(IServiceCollection services)
        {
            var profileValidator = new ProfileValidator(GetValidator(), GetLogger());
            services.Add(new ServiceDescriptor(typeof(IProfileValidator), profileValidator));
        }

        private static ILogger GetLogger()
        {
            var factory = new LoggerFactory();
            var logger = factory.CreateLogger("FhirStarter");
            return logger;
        }

        private static Validator GetValidator()
        {
            return ValidatorFactory.GetValidator();
        }

        private static void BindIFhirServices(IServiceCollection services, List<TypeInitializer> serviceTypes, Type classType)
        {
            var serviceType = FindType(serviceTypes, classType);
            if (serviceType != null)
            {
                if (serviceType.Name.Equals(nameof(IFhirService)))
                {
                    var instance = (IFhirService)Activator.CreateInstance(classType);
                   services.Add(new ServiceDescriptor(typeof(IFhirService), instance));
                    //app.Bind<IFhirService>().ToConstant(instance);
                }
                else if (serviceType.Name.Equals(nameof(IFhirMockupService)))
                {
                    var instance = (IFhirMockupService)Activator.CreateInstance(classType);
                    services.Add(new ServiceDescriptor(typeof(IFhirMockupService), instance));
                    //kernel.Bind<IFhirMockupService>().ToConstant(instance);
                }
                //else if (serviceType.Name.Equals(nameof(AbstractStructureDefinitionService)))
                //{
                //    var structureDefinitionService = (AbstractStructureDefinitionService)Activator.CreateInstance(classType);
                //  //  kernel.Bind<AbstractStructureDefinitionService>().ToConstant(structureDefinitionService);
                //    var validator = structureDefinitionService.GetValidator();
                //    if (validator != null)
                //    {
                //        var profileValidator = new ProfileValidator(validator);
                //        kernel.Bind<ProfileValidator>().ToConstant(profileValidator);
                //    }
                //    _amountOfIFhirStructureDefinitionsInitialized++;
                //}
                //var instance = (IProfileValidator)Activator.CreateInstance(typeof(ProfileValidator));
                //services.Add(new ServiceDescriptor(typeof(IProfileValidator), GetProfileValidator()));
            }
        }

       

        private static TypeInitializer FindType(List<TypeInitializer> serviceTypes, Type classType)
        {
            foreach (var service in serviceTypes)
            {
                if (service.ServiceType.IsAssignableFrom(classType) && !classType.IsInterface && !classType.IsAbstract)
                    return service;
            }
            return null;
        }

        #endregion Assembly
    }
}
