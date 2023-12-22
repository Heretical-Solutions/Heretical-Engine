using System;

namespace HereticalSolutions.LifetimeManagement
{
    public static class LifetimeSynchronizer
    {
        public static void SyncLifetimes(
            ILifetimeable target,
            ILifetimeable parent,
            object[] initializationArgs)
        {
            if (parent == null)
                return;

            Action initializationDelegate = () => target.Initialize(initializationArgs);

            SyncLifetimes(target, parent, initializationDelegate);
        }

        /// <summary>
        /// Attach target's custom initialization delegate to parent's OnInitialized, 
        /// target's OnCleanedUp to parent's OnCleanedUp 
        /// and target's TearDown to parent's OnTornDown
        /// </summary>
        /// <param name="target">The target lifetimable</param>
        /// <param name="parent">The target's parent lifetimeable</param>
        /// <param name="initializationDelegate">The target's custom initialization delegate</param>
        public static void SyncLifetimes(
            ILifetimeable target,
            ILifetimeable parent,
            Action initializationDelegate)
        {
            if (parent == null)
                return;
            
            parent.OnInitialized += initializationDelegate;
            parent.OnCleanedUp += target.Cleanup;

            Action desyncDelegate = null;

            // Event handler for parent's OnTornDown event
            desyncDelegate = () =>
            {
                parent.OnInitialized -= initializationDelegate;
                parent.OnCleanedUp -= target.Cleanup;

                parent.OnTornDown -= desyncDelegate;

                target.TearDown();
            };

            parent.OnTornDown += desyncDelegate;
        }
    }
}