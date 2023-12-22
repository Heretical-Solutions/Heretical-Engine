 using DefaultEcs;

 namespace HereticalSolutions.GameEntities
 {
     /// <summary>
     /// Represents a component for a prediction entity.
     /// </summary>
     [Component("Registry world")]
    [IdentityComponent]
    public struct PredictionEntityComponent
     {
         /// <summary>
         /// The prediction entity.
         /// </summary>
         public Entity PredictionEntity;

         /// <summary>
         /// The ID of the prototype.
         /// </summary>
         public string PrototypeID;
     }
 }