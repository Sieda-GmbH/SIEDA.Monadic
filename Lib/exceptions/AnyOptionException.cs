using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>
   /// Common superclass for exceptions thrown exclusively by <see cref="Option{TValue, TFail}"/> and <see cref="EOption{TValue}"/>.
   /// Is abstract due to is generic nature, classes actually implementating this proper are modelling concrete problems.
   /// </summary>
   /// <remarks> Useful for 'try-catch' clauses. </remarks>
   [Serializable]
   public abstract class AnyOptionException : Exception
   {
      /// <summary> Abstract CTOR </summary>
      protected AnyOptionException( string message ) : base( message ) {}

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public AnyOptionException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public AnyOptionException( string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected AnyOptionException( SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}