using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Saras.SystemFramework.Core.Logging;

namespace Saras.eMarking.RCScheduler.Jobs
{
    public abstract class BaseJobScheduleManager
    {
        # region "Private variables"

        private SarasLogger log = new SarasLogger();

        #endregion

        # region "Protected Properties"
        /// <summary>
        /// Logger Object 
        /// </summary>
        protected ISarasLogger Log
        {
            get
            {
                return log;
            }
        }

        #endregion

        public BaseJobScheduleManager()
        {
            ///Initialize Logger Object
            log.InitializeLogger(this.GetType());
        }


    }
}

