using System;
using NUnit.Framework;
using SIEDA.Monadic;
using SIEDA.MonadicTests.HelperClass;

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
         var testValue = EValidation.Success; //this must work ;-)
         Assert.That( testValue.IsSuccess, Is.True );
      }

      [Test]
      public void ConstructFailure_ErrorNull()
      {
         Assert.That( () => EValidation.Failure( null ), Throws.TypeOf<ValidationFailureConstructionException>() );
      }

      [Test]
      public void ConstructFailure_ErrorNotNull()
      {
         var testValue = EValidation.Failure( new ArgumentException() );

         Assert.That( testValue.IsSuccess, Is.False );
      }

      [Test]
      public void ConstructFailure_WrappingErrorGiven()
      {
         var testValue = EValidation.Wrapping<Exception>( () => new ArgumentException() );
         Assert.That( testValue.IsSuccess, Is.True );
      }

      [Test]
      public void ConstructFailure_WrappingErrorThrown()
      {
         var testValue = EValidation.Wrapping<Exception>( () => throw new ArgumentException() );
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
         var y = EValidation.Success;

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "EValidations sind equivalent zu Validations mit Right-Hand-Side Exception" )]
      public void Equals_Validation()
      {
         var aEValid = EValidation.Success;
         var aValid = Validation<Exception>.Success;

         var exception = new ArgumentException();
         var bEValid = EValidation.Failure( exception );
         var bValid = Validation<Exception>.Failure( exception );

         Assert.That( aEValid.GetHashCode(), Is.EqualTo( aValid.GetHashCode() ), "HashCode not correct (Success-Case)" );
         Assert.That( bEValid.GetHashCode(), Is.EqualTo( bValid.GetHashCode() ), "HashCode not correct (Failure-Case)" );

         Assert.That( aEValid, Is.EqualTo( aValid ), "EValidation-Equals is buggy! (Success-Case)" );
         Assert.That( bValid, Is.EqualTo( bEValid ), "EValidation-Equals is buggy! (Failure-Case)" );
         Assert.That( aValid, Is.EqualTo( aEValid ), "Implementation of Validation is not accepting EValidation! (Success-Case)" );
         Assert.That( bValid, Is.EqualTo( bEValid ), "Implementation of Validation is not accepting EValidation! (Failure-Case)" );

         Assert.That( aEValid, Is.Not.EqualTo( bValid ) ); //sanity-check
      }

      [Test]
      [Description( "EValidations sind nie equivalent zu Validations ohne Right-Hand-Side Exception" )]
      public void Equals_Validation_DifferentType()
      {
         var aEValid = EValidation.Success;
         var aValid = Validation<string>.Success;

         var exception = new ArgumentException();
         var bEValid = EValidation.Failure( exception );
         var bValid = Validation<string>.Failure( "whatever" );

         Assert.That( aEValid, Is.Not.EqualTo( aValid ) );
         Assert.That( bValid, Is.Not.EqualTo( bEValid ) );
         Assert.That( aValid, Is.Not.EqualTo( aEValid ) );
         Assert.That( bValid, Is.Not.EqualTo( bEValid ) );

         Assert.That( aEValid, Is.Not.EqualTo( bValid ) ); //sanity-check
      }

      [Test]
      [Description( "EValidations mit unterschiedlichen Exceptiontypen sind unterschiedlich, auch bei Subtypen im Failure-Fall!" )]
      public void Equals_Validation_DifferentType_SubtypeCase()
      {
         var sameText = "ANY_VALUE";
         var a = new MyExceptionWithTypicalEquals( sameText );
         var b = new MySubexceptionWithTypicalValueBasedEquals( sameText, 123 );
         Assert.That( a.Equals( b ), Is.True, "TEST-SETUP IS BROKEN!" );
         Assert.That( b.Equals( a ), Is.True, "TEST-SETUP IS BROKEN!" );

         var x = EValidation.Failure( a );
         var y = EValidation.Failure( b );

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
         Assert.Throws<ValidationNoFailureException>( () => EValidation.Success.FailureOrThrow() );
      }

      #endregion Accessing Value

      #region Convert
      
      [Test]
      [Description( "Success wird zu Option.None konvertiert" )]
      public void ConvertToOption_Success()
      {
         var validation = EValidation.Success;
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
         var validation = EValidation.Success;
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