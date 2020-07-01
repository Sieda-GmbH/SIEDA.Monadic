using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>
   /// Common superclass for exceptions thrown exclusively by <see cref="Failable{TValue, TFail}"/>.
   /// Is abstract due to is generic nature, classes actually implementating this proper model conrete problems.
   /// </summary>
   /// <remarks> Useful for 'try-catch' clauses. </remarks>
   [Serializable]
   public abstract class FailableException : AnyFailableException
   {
      /// <summary> Abstract CTOR </summary>
      protected FailableException( string message ) : base( message ) {}

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableException(string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected FailableException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}