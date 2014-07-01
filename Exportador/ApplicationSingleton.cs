using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.Unity;

namespace Exportador
{
    public sealed class ApplicationSingleton
    {
        private static readonly ApplicationSingleton _instance = new ApplicationSingleton();
        public UnityContainer Container { get; set; }
                
        private ApplicationSingleton()
        {
            Container = new UnityContainer();
        }

        public static ApplicationSingleton Instance
        {
            get { return _instance; }
        }

        public void ConfigureContainer()
        {
            Container.AddNewExtension<EnterpriseLibraryCoreExtension>();
        }

    }
}
