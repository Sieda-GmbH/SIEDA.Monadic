using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>
   /// Common superclass for exceptions thrown exclusively by <see cref="Validation{TFail}"/> and <see cref="EValidation"/>.
   /// Is abstract due to is generic nature, classes actually implementating this proper are modelling concrete problems.
   /// </summary>
   /// <remarks> Useful for 'try-catch' clauses. </remarks>
   [Serializable]
   public abstract class AnyValidationException : Exception
   {
      /// <summary> Abstract CTOR </summary>
      protected AnyValidationException( string message ) : base( message ) {}

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public AnyValidationException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public AnyValidationException( string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected AnyValidationException( SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}