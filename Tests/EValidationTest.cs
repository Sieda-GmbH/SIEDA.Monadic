using System;
using NUnit.Framework;
using SIEDA.Monadic;

namespace SIEDA.MonadicTests
{
   [TestFixture]
   [Description( "Prüft Verhalten von SIEDA's EValidation." )]
   public class EValidationTest
   {
      #region Construction

      [Test]
      public void ConstructSuccess()
      {
         var testValue = EValidation.Success(); //this must work ;-)
         Assert.That( testValue.IsSuccess, Is.True );
      }

      [Test]
      public void ConstructFailure_ErrorNull()
      {
         Assert.That( () => EValidation.Failure( null ), Throws.TypeOf<EValidationFailureConstructionException>() );
      }

      [Test]
      public void ConstructFailure_ErrorNotNull()
      {
         var testValue = EValidation.Failure( new ArgumentException() );

         Assert.That( testValue.IsSuccess, Is.False );
      }

      #endregion Construction

      #region Equals

      [Test]
      [Description( "Beide haben denselben Wert." )]
      public void Equals_FailureEqual()
      {
         var e = new ArgumentException();

         var x = EValidation.Failure( e );
         var y = EValidation.Failure( e );

         Assert.That( x.Equals( y ), Is.True );
      }

      [Test]
      [Description( "Beide haben unterschiedliche Werte." )]
      public void Equals_FailureInequal()
      {
         var e1 = new ArgumentException();
         var e2 = new ArgumentException();

         var x = EValidation.Failure( e1 );
         var y = EValidation.Failure( e2 );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Gescheitertes Validation ist ungleich jedem Erfolgreichen." )]
      public void Equals_FailedInequalToSuccess()
      {
         var x = EValidation.Failure( new ArgumentException() );
         var y = EValidation.Success();

         Assert.That( x.Equals( y ), Is.False );
      }

      #endregion Equals

      #region Accessing Value

      [Test]
      public void Error_ReturnsFailure()
      {
         var exception = new ArgumentNullException();
         Assert.That( EValidation.Failure( exception ).FailureOrThrow(), Is.EqualTo( exception ) );
      }

      [Test]
      public void OrThrowWithText_DoesNotThrowIfSuccess()
      {
         Assert.Throws<EValidationNoFailureException>( () => EValidation.Success().FailureOrThrow() );
      }

      #endregion Accessing Value

      #region Convert
      
      [Test]
      [Description( "Success wird zu Option.None konvertiert" )]
      public void ConvertToOption_Success()
      {
         var validation = EValidation.Success();
         var option = validation.ToOption();

         Assert.That( option.IsNone, Is.True );
      }

      [Test]
      [Description( "Success wird zu Option.Failure konvertiert" )]
      public void ConvertToOption_Failure()
      {
         var exception = new ArgumentNullException();
         var validation = EValidation.Failure( exception );
         var option = validation.ToOption();

         Assert.That( option.IsFailure, Is.True );
         Assert.That( option.FailureOrThrow(), Is.EqualTo( exception ) );
      }


      [Test]
      [Description( "Success wird zu EOption.None konvertiert" )]
      public void ConvertToEOption_Success()
      {
         var validation = EValidation.Success();
         var option = validation.ToEOption();

         Assert.That( option.IsNone, Is.True );
      }

      [Test]
      [Description( "Success wird zu EOption.Failure konvertiert" )]
      public void ConvertToEOption_Failure()
      {
         var exception = new ArgumentNullException();
         var validation = EValidation.Failure( exception );
         var option = validation.ToEOption();

         Assert.That( option.IsFailure, Is.True );
         Assert.That( option.FailureOrThrow(), Is.EqualTo( exception ) );
      }
      #endregion Convert
   }
}