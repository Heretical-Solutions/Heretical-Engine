
using HereticalSolutions.Collections;

using HereticalSolutions.Pools.Behaviours;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.GenericNonAlloc
{
    public class PackedArrayPool<T>: IFixedSizeCollection<IPoolElement<T>>, INonAllocPool<T>, IIndexable<IPoolElement<T>>, IModifiable<IPoolElement<T>[]>, ICountUpdateable
    {
        private readonly IFormatLogger logger;

        private IPoolElement<T>[] contents;
        
        private int count;

        private readonly IPushBehaviourHandler<T> pushBehaviourHandler;

        public PackedArrayPool(
            IPoolElement<T>[] contents,
            IFormatLogger logger)
        {
            this.contents = contents;

            this.logger = logger;
            
            count = 0;

            pushBehaviourHandler = new PushToINonAllocPoolBehaviour<T>(this);
        }
        
        #region IFixedSizeCollection
        
        /// <summary>
        /// Gets the capacity of the pool.
        /// </summary>
        public int Capacity { get { return contents.Length; } }
        
        /// <summary>
        /// Gets the element at the specified index in the pool.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>The element at the specified index.</returns>
        public IPoolElement<T> ElementAt(int index)
        {
	        return contents[index];
        }

        #endregion

		#region IModifiable

		/// <summary>
		/// Gets or sets the contents of the pool.
		/// </summary>
		public IPoolElement<T>[] Contents { get { return contents; } }
		
		/// <summary>
		/// Updates the contents of the pool.
		/// </summary>
		/// <param name="newContents">The updated contents of the pool.</param>
		public void UpdateContents(IPoolElement<T>[] newContents)
        {
            contents = newContents;
        }
		
		/// <summary>
		/// Updates the count of the pool.
		/// </summary>
		/// <param name="newCount">The updated count of the pool.</param>
		public void UpdateCount(int newCount)
		{
			count = newCount;
		}

		#endregion

		#region IIndexable

		/// <summary>
		/// Gets the count of the pool.
		/// </summary>
		public int Count { get { return count; } }
		
		public IPoolElement<T> this[int index]
		{
			get
			{
                if (index >= count || index < 0)
					logger?.ThrowException<PackedArrayPool<T>>(
						$"INVALID INDEX: {index} COUNT: {Count} CAPACITY: {Capacity}");

				return contents[index];
			}
		}
		
		public IPoolElement<T> Get(int index)
		{
			if (index >= count || index < 0)
				logger?.ThrowException<PackedArrayPool<T>>(
					$"INVALID INDEX: {index} COUNT: {count} CAPACITY: {Capacity}");

			return contents[index];
		}

        #endregion

        #region INonAllocPool

		/// <summary>
		/// Pops an item from the pool.
		/// </summary>
		/// <returns>The popped item.</returns>
        public IPoolElement<T> Pop()
        {
            var result = contents[count];

            
            //Update index
            result.Metadata.Get<IIndexed>().Index = count;

            
            //Update element data
            var elementAsPushable = (IPushable<T>)result; 
            
            elementAsPushable.Status = EPoolElementStatus.POPPED;
            
            elementAsPushable.UpdatePushBehaviour(pushBehaviourHandler);
            
            
            //Increase popped elements count
            count++;

            
            return result;
        }

		public IPoolElement<T> Pop(int index)
		{
            if (index < count)
            {
                logger?.ThrowException<PackedArrayPool<T>>(
                    $"ELEMENT AT INDEX {index} IS ALREADY POPPED");
			}


			int lastFreeItemIndex = count;

			if (index != lastFreeItemIndex)
			{
				IIndexed lastFreeItemAsIndexed = contents[lastFreeItemIndex].Metadata.Get<IIndexed>();
				
				IIndexed itemAtIndexAsIndexed = contents[index].Metadata.Get<IIndexed>();
				
				
				lastFreeItemAsIndexed.Index = -1;

				itemAtIndexAsIndexed.Index = index;


				//Rider offers 'swap via deconstruction' here. I dunno, this three liner is more familiar and readable to me somehow
				var swap = contents[index];

				contents[index] = contents[lastFreeItemIndex];

				contents[lastFreeItemIndex] = swap;
			}
			else
			{
				contents[index].Metadata.Get<IIndexed>().Index = index;
			}


			var result = contents[lastFreeItemIndex];

			
			//Update index
			result.Metadata.Get<IIndexed>().Index = count;

            
			//Update element data
			var elementAsPushable = (IPushable<T>)result; 
            
			elementAsPushable.Status = EPoolElementStatus.POPPED;
            
			elementAsPushable.UpdatePushBehaviour(pushBehaviourHandler);
			
			
			count++;

			return result;
		}

        /// <summary>
        /// Pushes an item back into the pool.
        /// </summary>
        /// <param name="item">The item to be pushed.</param>
        public void Push(IPoolElement<T> item)
        {
            Push(item.Metadata.Get<IIndexed>().Index);
        }

        /// <summary>
        /// Pushes an item back into the pool at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        public void Push(int index)
        {
            if (index >= count)
            {
	            return;
            }

            int lastItemIndex = count - 1;

            int resultIndex = index;

            if (index != lastItemIndex)
            {
	            IIndexed lastItemAsIndexed = contents[lastItemIndex].Metadata.Get<IIndexed>();
				
	            IIndexed itemAtIndexAsIndexed = contents[index].Metadata.Get<IIndexed>();
	            
	            
	            lastItemAsIndexed.Index = index;

	            itemAtIndexAsIndexed.Index = -1;


	            //Rider offers 'swap via deconstruction' here. I dunno, this three liner is more familiar and readable to me somehow
                var swap = contents[index];

                contents[index] = contents[lastItemIndex];

                contents[lastItemIndex] = swap;


                resultIndex = lastItemIndex;
            }
            else
            {
				contents[index].Metadata.Get<IIndexed>().Index = -1;
            }
            
            
            //Update element data
            var elementAsPushable = (IPushable<T>)contents[resultIndex]; 
            
            elementAsPushable.Status = EPoolElementStatus.PUSHED;
            
            elementAsPushable.UpdatePushBehaviour(null);
            

            count--;
        }

        /// <summary>
        /// Checks if the pool has free space for more items.
        /// </summary>
        public bool HasFreeSpace { get { return count < contents.Length; } }
        
        #endregion
    }
}