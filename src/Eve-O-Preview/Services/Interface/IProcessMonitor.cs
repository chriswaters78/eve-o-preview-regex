using System;
using System.Collections.Generic;

namespace EveOPreview.Services
{
	public interface IProcessMonitor
	{
		IProcessInfo GetMainProcess();
		ICollection<IProcessInfo> GetAllProcesses();
		void GetUpdatedProcesses(out ICollection<IProcessInfo> addedProcesses, out ICollection<IProcessInfo> updatedProcesses, out ICollection<IProcessInfo> removedProcesses);
		// Cycle groups are numbered starting from 1
		int? GetProcessOrder(IntPtr processHandle, int cycleGroup);
        List<(int Order, IntPtr Handle)> GetKnownProcessOrders(int cycleGroup, bool reverse);


    }
}