using System;
using NUnit.Framework;
using SIEDA.Monadic;

namespace SIEDA.MonadicTests
{
   [TestFixture]
   [Description( "Prüft Verhalten von SIEDA's Validation<E>." )]
   public class ValidationTest
   {
      #region Construction

      [Test]
      public void ConstructSuccess()
      {
         var testValue = Validation<object>.Success(); //this must work ;-)
         Assert.That( testValue.IsSuccess, Is.True );
      }

      [Test]
      public void ConstructFailure_ErrorNull()
      {
         Assert.That( () => Validation<object>.Failure( null ), Throws.TypeOf<ValidationFailureConstructionException>() );
      }

      [Test]
      public void ConstructFailure_ErrorNotNull()
      {
         var testValue = Validation<object>.Failure( new object() );

         Assert.That( testValue.IsSuccess, Is.False );
      }

      #endregion Construction

      #region ToString

      private class TestObj
      {
         private readonly string S;

         public TestObj( string s )
         {
            S = s;
         }

         public override string ToString() => $"Object '{S}'";
      }

      [Test]
      public void ToString_Failure()
      {
         Assert.That( Validation<TestObj>.Failure( new TestObj( "evil" ) ).ToString(), Is.EqualTo( "[Validation<TestObj>.Failure: Object 'evil']" ) );
      }

      #endregion ToString

      #region Equals

      [Test]
      [Description( "Beide haben denselben Wert." )]
      public void Equals_FailureEqual()
      {
         var t1 = new Tuple<string, int>( "abc", 123 );
         var t2 = new Tuple<string, int>( "abc", 123 );

         var x = Validation<Tuple<string, int>>.Failure( t1 );
         var y = Validation<Tuple<string, int>>.Failure( t2 );

         Assert.That( t1.Equals( t2 ), Is.True );
         Assert.That( x.Equals( y ), Is.True );
      }

      [Test]
      [Description( "Beide haben unterschiedliche Werte." )]
      public void Equals_FailureInequal()
      {
         var t1 = new Tuple<string, int>( "abc", 123 );
         var t2 = new Tuple<string, int>( "dfs", 532 );

         var x = Validation<Tuple<string, int>>.Failure( t1 );
         var y = Validation<Tuple<string, int>>.Failure( t2 );

         Assert.That( t1.Equals( t2 ), Is.False );
         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Gescheitertes Validation ist ungleich jedem Erfolgreichen." )]
      public void Equals_FailedInequalToSuccess()
      {
         var x = Validation<string>.Failure( "how appaling!" );
         var y = Validation<string>.Success();
         var z = Validation<Validation<string>>.Failure( y );

         Assert.That( x.Equals( y ), Is.False );
         Assert.That( x.Equals( z ), Is.False );
      }

      [Test]
      [Description( "Validations zu unterschiedlichen Typen sind unterschiedlich." )]
      public void Equals_DifferentTypeInequal()
      {
         var x = Validation<Tuple<string, int>>.Success();
         var y = Validation<Tuple<string, string>>.Success();
         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Validations zu unterschiedlichen Typen sind unterschiedlich - für Failure wird keine Ausnahme gemacht." )]
      public void Equals_DifferentTypeInequalForFailure()
      {
         var t1 = new Tuple<string, int>( "abc", 123 );
         var t2 = new Tuple<string, string>( "abc", "123" );

         var x = Validation<Tuple<string, int>>.Failure(t1);
         var y = Validation<Tuple<string, string>>.Failure(t2);

         Assert.That( t1.Equals( t2 ), Is.False );
         Assert.That( x.Equals( y ), Is.False );
      }

      #endregion Equals

      #region Accessing Value

      [Test]
      public void Error_ReturnsFailure()
      {
         Assert.That( Validation<int>.Failure( -1 ).FailureOrThrow(), Is.EqualTo( -1 ) );
      }

      [Test]
      public void OrThrowWithText_DoesNotThrowIfSuccess()
      {
         Assert.Throws<ValidationNoFailureException>( () => Validation<int>.Success().FailureOrThrow() );
      }

      #endregion Accessing Value

      #region Convert
      
      [Test]
      [Description( "Success wird zu Option.None konvertiert" )]
      public void ConvertToOption_Success()
      {
         var Validation = Validation<string>.Success();
         var option = Validation.ToOption();

         Assert.That( option.IsNone, Is.True );
      }

      [Test]
      [Description( "Success wird zu Option.Failure konvertiert" )]
      public void ConvertToOption_Failure()
      {
         var Validation = Validation<string>.Failure("hallo");
         var option = Validation.ToOption();

         Assert.That( option.IsFailure, Is.True );
         Assert.That( option.FailureOrThrow(), Is.EqualTo( "hallo" ) );
      }

      #endregion Convert
   }
}