using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="EFailable{TValue}"/> on illegal instance construction using a given delegate.</summary>
   [Serializable]
   public class FailableFromWrappedConstructionException : AnyFailableException
   {
      /// <summary>Allows <see cref="EFailable{TValue}"/> to construct and throw this exception.</summary>
      internal FailableFromWrappedConstructionException( Type typeValue )
         : base( $"Illegal 'success' given for EFailable<{typeValue.Name}>, the given function must not be a 'null'-reference!" ) { }

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableFromWrappedConstructionException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableFromWrappedConstructionException( string message ) : base( message ) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableFromWrappedConstructionException( string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected FailableFromWrappedConstructionException( SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}