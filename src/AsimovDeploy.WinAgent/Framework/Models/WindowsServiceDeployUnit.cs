/*******************************************************************************
* Copyright (C) 2012 eBay Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
******************************************************************************/

using System.ServiceProcess;
using AsimovDeploy.WinAgent.Framework.Common;
using AsimovDeploy.WinAgent.Framework.Deployment.Steps;
using AsimovDeploy.WinAgent.Framework.Tasks;

namespace AsimovDeploy.WinAgent.Framework.Models
{
    public class WindowsServiceDeployUnit : DeployUnit
    {
        public string ServiceName { get; set; }
        public string Url { get; set; }

        public override AsimovTask GetDeployTask(AsimovVersion version, ParameterValues parameterValues)
        {
            var task = new DeployTask(this, version, parameterValues);
            task.AddDeployStep<UpdateWindowsService>();
            return task;
        }

        public override AsimovTask GetVerifyTask()
        {
            return new NoOpTask();
        }

        public override DeployUnitInfo GetUnitInfo()
        {
            var serviceManager = new ServiceController(ServiceName);

            var unitInfo = base.GetUnitInfo();
            unitInfo.Url = Url;

            try
            {
                unitInfo.Status = serviceManager.Status == ServiceControllerStatus.Running ? UnitStatus.Running : UnitStatus.Stopped;
                unitInfo.Info = string.Format("Last deployed: {0}", unitInfo.Version.DeployTimestamp);
            }
            catch
            {
                unitInfo.Status = UnitStatus.NotFound;
                unitInfo.Info = "";
            }

            return unitInfo;
        }
    }
}