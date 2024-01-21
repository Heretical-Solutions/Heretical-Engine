namespace HereticalSolutions.LifetimeManagement
{
    public static class LifetimeSynchronizer
    {
        public static void SyncLifetimes(
            ILifetimeable target,
            ILifetimeable parent,
            object[] initializationArgs = null,
            bool initializeIfParentInitialized = true)
        {
            if (target == null)
                return;

            if (parent == null)
                return;

            var targetAsInitializable = target as IInitializable;

            if (targetAsInitializable == null)
                return;

            Action initializationDelegate = () => targetAsInitializable.Initialize(initializationArgs);

            SyncLifetimes(
                target,
                parent,
                initializationDelegate,
                initializeIfParentInitialized);
        }

        public static void SyncLifetimes(
            ILifetimeable target,
            ILifetimeable parent,
            Action initializationDelegate,
            bool initializeIfParentInitialized = true)
        {
            if (target == null)
                return;

            if (parent == null)
                return;

            if (initializationDelegate != null)
                parent.OnInitialized += initializationDelegate;

            var targetAsCleanUppable = target as ICleanUppable;

            if (targetAsCleanUppable != null)
                parent.OnCleanedUp += targetAsCleanUppable.Cleanup;

            var targetAsTearDownable = target as ITearDownable;

            Action desyncDelegate = null;

            // Event handler for parent's OnTornDown event
            desyncDelegate = () =>
            {
                if (initializationDelegate != null)
                    parent.OnInitialized -= initializationDelegate;

                if (targetAsCleanUppable != null)
                    parent.OnCleanedUp -= targetAsCleanUppable.Cleanup;

                parent.OnTornDown -= desyncDelegate;

                if (targetAsTearDownable != null)
                {
                    targetAsTearDownable.TearDown();
                }
            };

            parent.OnTornDown += desyncDelegate;

            if (initializeIfParentInitialized
                && parent.IsInitialized
                && initializationDelegate != null)
                initializationDelegate();
        }

        public static void SyncEndOfLifetimes(
            ILifetimeable target,
            ILifetimeable parent)
        {
            if (target == null)
                return;

            if (parent == null)
                return;

            var targetAsCleanUppable = target as ICleanUppable;

            if (targetAsCleanUppable != null)
                parent.OnCleanedUp += targetAsCleanUppable.Cleanup;

            var targetAsTearDownable = target as ITearDownable;

            Action desyncDelegate = null;

            // Event handler for parent's OnTornDown event
            desyncDelegate = () =>
            {
                if (targetAsCleanUppable != null)
                    parent.OnCleanedUp -= targetAsCleanUppable.Cleanup;

                parent.OnTornDown -= desyncDelegate;

                if (targetAsTearDownable != null)
                {
                    targetAsTearDownable.TearDown();
                }
            };

            parent.OnTornDown += desyncDelegate;
        }
    }
}