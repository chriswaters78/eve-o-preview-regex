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

		// Cycle groups are numbered starting from 1 both in the config file and in the public API
		private const int CYCLE_GROUP_COUNT = 2;
        #endregion

        #region Private fields
        // The place a client takes in every cycle group is resolved once and then cached here
        // The regex lookups it requires are far too expensive to be repeated on every hotkey press
        private readonly IDictionary<IntPtr, (int?[] Orders, string Title)> _processCache;
        private IProcessInfo _currentProcessInfo;
        private readonly IThumbnailConfiguration _configuration;

        #endregion

        public ProcessMonitor(IThumbnailConfiguration configuration)
		{
			this._processCache = new Dictionary<IntPtr, (int?[], string)>(512);
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

		public int? GetProcessOrder(IntPtr processHandle, int cycleGroup)
		{
			if (!ProcessMonitor.IsKnownCycleGroup(cycleGroup))
			{
				return null;
			}

			if (this._processCache.TryGetValue(processHandle, out (int?[] Orders, string Title) cachedProcess))
			{
				return cachedProcess.Orders[cycleGroup - 1];
			}

			return null;
		}

		public List<(int Order, IntPtr Handle)> GetKnownProcessOrders(int cycleGroup, bool reverse)
		{
			if (!ProcessMonitor.IsKnownCycleGroup(cycleGroup))
			{
				return new List<(int, IntPtr)>();
			}

			int groupIndex = cycleGroup - 1;

			return this._processCache.Where(kvp => kvp.Value.Orders[groupIndex] != null)
				.Select(kvp => (Order: kvp.Value.Orders[groupIndex].Value, Handle: kvp.Key))
				.OrderBy(process => (reverse ? -1 : 1) * process.Order)
				.ToList();
		}

        public ICollection<IProcessInfo> GetAllProcesses()
		{
			ICollection<IProcessInfo> result = new List<IProcessInfo>(this._processCache.Count);

			// TODO Lock list here just in case
			foreach (KeyValuePair<IntPtr, (int?[] Orders, string Title)> entry in this._processCache)
			{
				result.Add(new ProcessInfo(entry.Key, entry.Value.Title));
			}

			return result;
		}

        private static bool IsKnownCycleGroup(int cycleGroup)
        {
            return (cycleGroup >= 1) && (cycleGroup <= ProcessMonitor.CYCLE_GROUP_COUNT);
        }

        private Dictionary<string, int> GetCycleGroupClientsOrder(int cycleGroup)
        {
            switch (cycleGroup)
            {
                case 1:
                    return this._configuration.CycleGroup1ClientsOrder;
                case 2:
                    return this._configuration.CycleGroup2ClientsOrder;
                default:
                    return null;
            }
        }

        // Resolves the place this client takes in each of the cycle groups
        private int?[] getMatchingCycleOrders(string windowTitle)
        {
            return this.updateMatchingCycleOrders(new int?[ProcessMonitor.CYCLE_GROUP_COUNT], windowTitle);
        }

        // A client keeps the place it was given when it was first matched into a cycle group
        // so only the groups it does not belong to yet are looked up again
        private int?[] updateMatchingCycleOrders(int?[] orders, string windowTitle)
        {
            for (int groupIndex = 0; groupIndex < orders.Length; groupIndex++)
            {
                if (orders[groupIndex] == null)
                {
                    orders[groupIndex] = this.getMatchingCycleOrder(groupIndex + 1, windowTitle);
                }
            }

            return orders;
        }

        private int? getMatchingCycleOrder(int cycleGroup, string windowTitle)
        {
            Dictionary<string, int> clientsOrder = this.GetCycleGroupClientsOrder(cycleGroup);

            if (clientsOrder == null)
            {
                return null;
            }

            var matchingOrders = clientsOrder.Where(co => new Regex(co.Key).IsMatch(windowTitle))
                .Select(co => co.Value).Distinct().ToArray();
            if (matchingOrders.Length > 1)
            {
                throw new Exception($"Found more than one matching order in cycle group {cycleGroup} for window '{windowTitle}'");
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


                this._processCache.TryGetValue(mainWindowHandle, out (int?[] orders, string title) cachedProcess);

				if (cachedProcess.title == null)
				{
                    this._processCache.Add(mainWindowHandle, (this.getMatchingCycleOrders(mainWindowTitle), mainWindowTitle));
                    // This is a new process in the list
                    // see if we can assign it an order
                    addedProcesses.Add(new ProcessInfo(mainWindowHandle, mainWindowTitle));
				}
				else
				{
					// This is an already known process
					if (cachedProcess.title != mainWindowTitle)
					{
						// The client was most probably sitting on the login screen when it was last seen
						// so the cycle groups it is still missing from are matched against its new title
						this._processCache[mainWindowHandle] = (this.updateMatchingCycleOrders(cachedProcess.orders, mainWindowTitle), mainWindowTitle);
                        updatedProcesses.Add(new ProcessInfo(mainWindowHandle, mainWindowTitle));
					}

					knownProcesses.Remove(mainWindowHandle);
				}
			}

			foreach (IntPtr index in knownProcesses)
			{
				(int?[] orders, string title) = this._processCache[index];
				removedProcesses.Add(new ProcessInfo(index, title));
				this._processCache.Remove(index);
			}
		}
	}
}
