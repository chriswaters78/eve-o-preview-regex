using EveOPreview.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace EveOPreview.Services.Implementation
{
	sealed class ProcessMonitor : IProcessMonitor
	{
		#region Private constants
		private const string DEFAULT_PROCESS_NAME = "ExeFile";
		private const string CURRENT_PROCESS_NAME = "EVE-O Preview";
        #endregion

        #region Private fields
        private readonly IDictionary<IntPtr, (int?, string)> _processCache;
        private IProcessInfo _currentProcessInfo;
        private readonly IThumbnailConfiguration _configuration;

        #endregion

        public ProcessMonitor(IThumbnailConfiguration configuration)
		{
			this._processCache = new Dictionary<IntPtr, (int?, string)>(512);
            this._configuration = configuration;

            // This field cannot be initialized properly in constructor
            // At the moment this code is executed the main application window is not yet initialized
            this._currentProcessInfo = new ProcessInfo(IntPtr.Zero, "");
		}

		private bool IsMonitoredProcess(string processName)
		{
			// This is a possible extension point
			return String.Equals(processName, ProcessMonitor.DEFAULT_PROCESS_NAME, StringComparison.OrdinalIgnoreCase);
		}

		private IProcessInfo GetCurrentProcessInfo()
		{
			var currentProcess = Process.GetCurrentProcess();
			return new ProcessInfo(currentProcess.MainWindowHandle, currentProcess.MainWindowTitle);
		}

		public IProcessInfo GetMainProcess()
		{
			if (this._currentProcessInfo.Handle == IntPtr.Zero)
			{
				var processInfo = this.GetCurrentProcessInfo();

				// Are we initialized yet?
				if (processInfo.Title != "")
				{
					this._currentProcessInfo = processInfo;
				}
			}

			return this._currentProcessInfo;
		}

		public ICollection<IProcessInfo> GetAllProcesses()
		{
			ICollection<IProcessInfo> result = new List<IProcessInfo>(this._processCache.Count);

			// TODO Lock list here just in case
			foreach (KeyValuePair<IntPtr, (int?, string)> entry in this._processCache)
			{
				result.Add(new ProcessInfo(entry.Key, entry.Value.Item2));
			}

			return result;
		}

        private int? getMatchingCycleOrder(string windowTitle)
        {
            var matchingOrders = _configuration.CycleGroup1ClientsOrder.Where(co => new Regex(co.Key).IsMatch(windowTitle))
                .Select(co => co.Value).Distinct().ToArray();
            if (matchingOrders.Length > 1)
            {
                throw new Exception($"Found more than one matching order for window '{windowTitle}'");
            }

            if (matchingOrders.Length > 0)
            {
                return matchingOrders[0];
            }
            return null;
        }


        public void GetUpdatedProcesses(out ICollection<IProcessInfo> addedProcesses, out ICollection<IProcessInfo> updatedProcesses, out ICollection<IProcessInfo> removedProcesses)
		{
			addedProcesses = new List<IProcessInfo>(16);
			updatedProcesses = new List<IProcessInfo>(16);
			removedProcesses = new List<IProcessInfo>(16);

			IList<IntPtr> knownProcesses = new List<IntPtr>(this._processCache.Keys);
			foreach (Process process in Process.GetProcesses())
			{
				string processName = process.ProcessName;

				if (!this.IsMonitoredProcess(processName))
				{
					continue;
				}

				IntPtr mainWindowHandle = process.MainWindowHandle;
				if (mainWindowHandle == IntPtr.Zero)
				{
					continue; // No need to monitor non-visual processes
				}

				string mainWindowTitle = process.MainWindowTitle;


                this._processCache.TryGetValue(mainWindowHandle, out (int? order, string title) cachedProcess);

				if (cachedProcess.title == null)
				{
                    this._processCache.Add(mainWindowHandle, (getMatchingCycleOrder(mainWindowTitle), mainWindowTitle));
                    // This is a new process in the list
                    // see if we can assign it an order
                    addedProcesses.Add(new ProcessInfo(mainWindowHandle, mainWindowTitle));
				}
				else
				{
					// This is an already known process
					if (cachedProcess.title != mainWindowTitle)
					{
						if (cachedProcess.order == null)
						{
							this._processCache[mainWindowHandle] = (getMatchingCycleOrder(mainWindowTitle), mainWindowTitle);
						}
						else
						{
                            this._processCache[mainWindowHandle] = (cachedProcess.order, mainWindowTitle);
                        }
                        updatedProcesses.Add(new ProcessInfo(mainWindowHandle, mainWindowTitle));
					}

					knownProcesses.Remove(mainWindowHandle);
				}
			}

			foreach (IntPtr index in knownProcesses)
			{
				(int? order, string title) = this._processCache[index];
				removedProcesses.Add(new ProcessInfo(index, title));
				this._processCache.Remove(index);
			}
		}
	}
}
