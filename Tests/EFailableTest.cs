using System;
using SIEDA.MonadicTests.HelperClass;
using NUnit.Framework;
using SIEDA.Monadic;

namespace SIEDA.MonadicTests
{
   [TestFixture]
   [Description( "Prüft Verhalten von SIEDA's EFailable<T,E>." )]
   public class EFailableTest
   {
      #region Construction

      [Test]
      public void ConstructSuccess_Null()
      {
         Assert.That( () => EFailable<object>.Success( null ), Throws.TypeOf<FailableSuccessConstructionException>() );
      }

      [Test]
      public void ConstructSuccess_NotNull()
      {
         var testValue = EFailable<object>.Success( new object() );

         Assert.That( testValue.IsSuccess, Is.True );
      }

      [Test]
      public void ConstructFailure_ErrorNull()
      {
         Assert.That( () => EFailable<object>.Failure( null ), Throws.TypeOf<FailableFailureConstructionException>() );
      }

      [Test]
      public void ConstructFailure_ErrorNotNull()
      {
         var testValue = EFailable<object>.Failure( new ArgumentException() );

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
      public void ToString_Success()
      {
         Assert.That( EFailable<TestObj>.Success( new TestObj("hallo") ).ToString(), Is.EqualTo( "[EFailable<TestObj>.Success: Object 'hallo']" ) );
      }

      [Test]
      public void ToString_Failure()
      {
         Assert.That( EFailable<TestObj>.Failure( new ArgumentException("evil") ).ToString(), Is.EqualTo( "[EFailable<TestObj>.Failure: System.ArgumentException: evil]" ) );
      }

      #endregion ToString

      #region Equals

      [Test]
      [Description( "Beide haben denselben Wert." )]
      public void Equals_SuccessEqual()
      {
         var t1 = new Tuple<string, int>( "abc", 123 );
         var t2 = new Tuple<string, int>( "abc", 123 );

         var x = EFailable<Tuple<string, int>>.Success( t1 );
         var y = EFailable<Tuple<string, int>>.Success( t2 );

         Assert.That( t1.Equals( t2 ), Is.True );
         Assert.That( x.Equals( y ), Is.True );
      }

      [Test]
      [Description( "Beide haben Werte, aber unterschiedlich." )]
      public void Equals_SuccessInequal()
      {
         var x = EFailable<int>.Success( 4 );
         var y = EFailable<int>.Success( 5 );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Beide haben denselben Fehler, aber es ist nicht die gleiche Instanz!" )]
      public void Equals_FailedEqualObjects()
      {
         var x = EFailable<int>.Failure( new ArgumentException() );
         var y = EFailable<int>.Failure( new ArgumentException() );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Beide haben eine gemeinsame Fehlerinstanz!" )]
      public void Equals_FailedSameInstance()
      {
         var e = new ArgumentException();
         var x = EFailable<int>.Failure( e );
         var y = EFailable<int>.Failure( e );

         Assert.That( x.Equals( y ), Is.True );
      }

      [Test]
      [Description( "Es ist NICHT egal, welches Problem ein gescheitertes EFailable hat" )]
      public void Equals_FailedInequal()
      {
         var x = EFailable<object>.Failure( new ArgumentException("what a disappointment") );
         var y = EFailable<object>.Failure( new ArgumentException("yet another disappointment") );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Gescheitertes EFailable ist ungleich jedem Erfolgreichen." )]
      public void Equals_FailedInequalToSuccess()
      {
         var x = EFailable<int>.Failure( new ArgumentException("how appaling!") );
         var y = EFailable<int>.Success( 0 );
         var z = EFailable<EFailable<int>>.Success( y );

         Assert.That( x.Equals( y ), Is.False );
         Assert.That( x.Equals( z ), Is.False );
      }

      [Test]
      [Description( "EFailables zu unterschiedlichen Typen sind unterschiedlich." )]
      public void Equals_DifferentTypeInequal()
      {
         var t1 = new Tuple<string, int>( "abc", 123 );
         var t2 = new Tuple<string, string>( "abc", "123" );

         var x = EFailable<Tuple<string, int>>.Success( t1 );
         var y = EFailable<Tuple<string, string>>.Success( t2 );

         Assert.That( t1.Equals( t2 ), Is.False );
         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "EFailables zu unterschiedlichen Typen sind unterschiedlich - für Failure wird keine Ausnahme gemacht." )]
      public void Equals_DifferentTypeInequalForFailure()
      {
         var x = EFailable<int>.Failure( new ArgumentException("horrible") );
         var y = EFailable<object>.Failure( new ArgumentException("horrible") );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "EFailables sind equivalent zu Failables mit Right-Hand-Side Exception" )]
      public void Equals_Failable()
      {
         var aEFail = EFailable<int>.Success( 4 );
         var aFail = Failable<int, Exception>.Success( 4 );

         var exception = new ArgumentException();
         var bEFail = EFailable<int>.Failure( exception );
         var bFail = Failable<int, Exception>.Failure( exception );

         Assert.That( aEFail.GetHashCode(), Is.EqualTo( aFail.GetHashCode() ), "HashCode not correct (Success-Case)" );
         Assert.That( bEFail.GetHashCode(), Is.EqualTo( bFail.GetHashCode() ), "HashCode not correct (Failure-Case)" );

         Assert.That( aEFail, Is.EqualTo( aFail), "EFailable-Equals is buggy! (Success-Case)" );
         Assert.That( bEFail, Is.EqualTo( bFail ), "EFailable-Equals is buggy! (Failure-Case)" );
         Assert.That( aFail, Is.EqualTo( aEFail ), "Implementation of Failable is not accepting EFailable! (Success-Case)" );
         Assert.That( bFail, Is.EqualTo( bEFail ), "Implementation of Failable is not accepting EFailable! (Failure-Case)" );

         Assert.That( aEFail, Is.Not.EqualTo( bFail ) ); //sanity-check
      }

      [Test]
      [Description( "EFailables sind nie equivalent zu Failables ohne Right-Hand-Side Exception" )]
      public void Equals_Failable_DifferentType()
      {
         var aEFail = EFailable<int>.Success( 4 );
         var aFail = Failable<int, string>.Success( 4 );

         var exception = new ArgumentException();
         var bEFail = EFailable<int>.Failure( exception );
         var bFail = Failable<int, string>.Failure( "whatever" );

         Assert.That( aEFail, Is.Not.EqualTo( aFail ) );
         Assert.That( bEFail, Is.Not.EqualTo( bFail ) );
         Assert.That( aFail, Is.Not.EqualTo( aEFail ) );
         Assert.That( bFail, Is.Not.EqualTo( bEFail ) );
      }

      #endregion Equals

      #region Accessing Value

      [Test]
      public void Or_FailureOrNull()
      {
         Assert.That( EFailable<object>.Failure( new ArgumentException() ).Or( null ), Is.Null );
      }

      [Test]
      public void Or_FailureOrValue()
      {
         Assert.That( EFailable<int>.Failure( new ArgumentException() ).Or( 1 ), Is.EqualTo( 1 ) );
      }

      [Test]
      public void Or_Value()
      {
         Assert.That( EFailable<int>.Success( 1 ).Or( 2 ), Is.EqualTo( 1 ) );
      }

      [Test]
      public void Error_ThrowsIfSuccess()
      {
         Assert.Throws<FailableNoFailureException>( () => EFailable<string>.Success( "HAPPY" ).FailureOrThrow() );
      }

      [Test]
      public void Error_ReturnsFailure()
      {
         var expected = new ArgumentNullException();
         Assert.That( EFailable<string>.Failure( expected ).FailureOrThrow(), Is.EqualTo( expected ) );
      }

      [Test]
      public void OrThrow_ThrowsNotInnerExceptionIfFailure()
      {
         Assert.Throws<FailableFailureException>( () => EFailable<string>.Failure( new ArgumentOutOfRangeException() ).OrThrow() );
      }

      [Test]
      public void OrThrow_DoesNotThrowIfSuccess()
      {
         var testValue = ""; //to be overwritten by "happy"

         Assert.DoesNotThrow( () => testValue = EFailable<string>.Success( "HAPPY" ).OrThrow() );
         Assert.That( testValue, Is.EqualTo( "HAPPY" ) );
      }

      #endregion Accessing Value

      #region Subtypes

      [Test]
      public void OrWithSubtype_Success()
      {
         var myClass = new MyClass();
         var mySubclass = new MySubclass();

         var underTest = EFailable<MyClass>.Success( myClass );
         var actualValue = underTest.Or( mySubclass );

         Assert.That( actualValue, Is.SameAs( myClass ) );
      }

      [Test]
      public void OrWithSubtype_Failure()
      {
         var mySubclass = new MySubclass();

         var underTest = EFailable<MyClass>.Failure( new ArgumentException( "irrelevant" ) );
         var actualValue = underTest.Or( mySubclass );

         Assert.That( actualValue, Is.SameAs( mySubclass ) );
      }

      [Test]
      public void OrWithParallelType_Success()
      {
         var mySubclass = new MySubclass();
         var myOtherSubclass = new MyOtherSubclass();

         var underTest = EFailable<MyClass>.Success( mySubclass );
         var actualValue = underTest.Or( myOtherSubclass );

         Assert.That( actualValue, Is.SameAs( mySubclass ) );
      }

      [Test]
      public void OrWithParallelType_Failure()
      {
         var myOtherSubclass = new MyOtherSubclass();

         var underTest = EFailable<MyClass>.Failure( new ArgumentException( "irrelevant" ) );
         var actualValue = underTest.Or( myOtherSubclass );

         Assert.That( actualValue, Is.SameAs( myOtherSubclass ) );
      }

      #endregion Subtypes

      #region Map and Nesting
      [Test]
      [Description( "Die Map-Operation wird auf Erfolge angewendet." )]
      public void Map_Success()
      {
         var original = EFailable<string>.Success( "hallo" );
         var result = original.Map( s => s+= " welt" );
         Assert.That( result.OrThrow, Is.EqualTo( "hallo welt" ) );
      }

      [Test]
      [Description( "Die Map-Operation wird nicht auf Fehlschläge angewendet." )]
      public void Map_Failure()
      {
         var e = new ArgumentException();
         var original = EFailable<string>.Failure( e );
         var result = original.Map( s => s+= " welt" );
         Assert.That( result.FailureOrThrow, Is.EqualTo( e ) );
      }

      [Test]
      [Description( "Die Map-Operation ist kompatibel mit Verschachtelung" )]
      public void Map_NestingInMap()
      {
         var flag = EFailable<bool>.Success( true );

         var result = flag.Map( _ => EFailable<bool>.Success( true ) );

         Assert.That( result.Or( EFailable<bool>.Failure( new ArgumentException() ) ).Or( false ), Is.True );
      }

      [Test]
      [Description( "Verschachtelte Fallunterscheidungen mit FlatMap." )]
      public void Map_NestingInFlatMap()
      {
         var flag1 = EFailable<bool>.Success( true );
         var flag2 = EFailable<bool>.Success( true );

         var result = flag1.FlatMap( outerBoolVal => flag2.Map( boolVal => boolVal && outerBoolVal ? 3 : 2 ) );

         Assert.That( result.Or( -999 ), Is.EqualTo( 3 ) );
      }

      [Test]
      [Description( "Verschachtelte Fallunterscheidungen mit Wertübergabe mit FlatMap." )]
      public void ValuePropagationInFlatMap()
      {
         var hallo = EFailable<string>.Success( "hallo" );
         var sp = EFailable<string>.Success( " " );

         var result = hallo.FlatMap( h => sp.Map( space => h + space + "welt" ) );

         Assert.That( result.Or( "nix da" ), Is.EqualTo( "hallo welt" ) );
      }

      [Test]
      [Description( "Map hat keine Probleme mit Typveränderung, weder zur Lauf- noch zur Compilezeit." )]
      public void MapToDifferentType()
      {
         var one = EFailable<int>.Success( 1 );
         EFailable<string> onePlusOne = one.Map( i => $"{i}+1=2" );

         Assert.That( onePlusOne.OrThrow(), Is.EqualTo( "1+1=2" ) );
      }

      #endregion Map and Nesting

      #region Is

      [Test]
      [Description( "Check mittels Is_ - Vergleich liefert 'true'" )]
      public void Is_DefinedTrue()
      {
         var f = EFailable<string>.Success( "hubba" );

         Assert.That( f.Is( "hubba" ), Is.EqualTo( true ) );
      }

      [Test]
      [Description( "Check mittels Is_ - Vergleich liefert 'false'" )]
      public void Is_DefinedFalse()
      {
         var f = EFailable<string>.Success( "hubba" );

         Assert.That( f.Is( "whatever" ), Is.EqualTo( false ) );
      }

      [Test]
      [Description( "Check mittels Is_ - None immer 'false'" )]
      public void Is_FalseWhenNone()
      {
         var f = EFailable<string>.Failure(new ArgumentException());

         Assert.That( f.Is( "hubba" ), Is.EqualTo( false ) );
      }

      #endregion Is

      #region Holds

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich liefert 'true'" )]
      public void Is_PredicateMatchingDefined()
      {
         var EFailable = EFailable<string>.Success( "hubba" );

         Assert.That( EFailable.Holds( s => s.Equals( "hubba" ) ), Is.EqualTo( true ) );
                                                   
      }

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich liefert 'false'" )]
      public void Is_PredicateNonMatchingDefined()
      {
         var EFailable = EFailable<string>.Success( "hubba" );

         Assert.That( EFailable.Holds( s => s.Equals( "hubba-hub" ) ), Is.EqualTo( false ) );
                                                   
      }

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich findet nicht statt" )]
      public void Is_PredicateUndefined()
      {
         var EFailable = EFailable<string>.Failure( new ArgumentException() );

         Assert.That( EFailable.Holds( s => s.Equals( "hubba-hub" ) ), Is.EqualTo( false ) );
                                                   
      }

      #endregion Holds

      #region TryGet

      [Test]
      [Description( "TryGetValue() produziert korrektes Boolean-Result bei Failure" )]
      public void TryGet_Result_Failure()
      {
         var EFailable = EFailable<string>.Failure( new ArgumentException() );

         Assert.IsFalse( EFailable.TryGetValue( out var s ) );
      }

      [Test]
      [Description( "TryGetValue() schreibt keinen Wert bei Failure" )]
      public void TryGet_Value_Failure()
      {
         var EFailable = EFailable<string>.Failure( new ArgumentException() );
         EFailable.TryGetValue( out var s );

         Assert.IsNull( s );
      }

      [Test]
      [Description( "TryGetValue() produziert korrektes Boolean-Result bei Success" )]
      public void TryGet_Value_Success()
      {
         var EFailable = EFailable<string>.Success( "blub" );

         Assert.IsTrue( EFailable.TryGetValue( out var s ) );
      }

      [Test]
      [Description( "TryGetValue() schreibt keinen Wert bei Success" )]
      public void TryGet_Result_Success()
      {
         var EFailable = EFailable<string>.Success( "blub" );
         EFailable.TryGetValue( out var s );

         Assert.That( s, Is.EqualTo( "blub" ) );
      }

      #endregion TryGet

      #region Convert
      
      [Test]
      [Description( "Success wird zu Maybe.Some konvertiert" )]
      public void ConvertToMaybe_Success()
      {
         var EFailable = EFailable<string>.Success( "hallo" );
         var maybe = EFailable.ToMaybe();

         Assert.That( maybe.IsSome, Is.True );
         Assert.That( maybe.OrThrow, Is.EqualTo( "hallo" ) );
      }

      [Test]
      [Description( "Failure wird zu Maybe.None konvertiert" )]
      public void ConvertToMaybe_Failure()
      {
         var EFailable = EFailable<string>.Failure( new ArgumentException() );
         var maybe = EFailable.ToMaybe();

         Assert.That( maybe.IsNone, Is.True );
      }

      [Test]
      [Description( "Failure wird zu Option.Failure konvertiert" )]
      public void ConvertToOption_Failure()
      {
         var EFailable = EFailable<string>.Failure( new ArgumentException( "abc" ) );
         var option = EFailable.ToOption();

         Assert.That( option.IsFailure, Is.True );
      }

      [Test]
      [Description( "Success wird zu Option.Some konvertiert" )]
      public void ConvertToOption_Success()
      {
         var EFailable = EFailable<string>.Success( "hubba" );
         var option = EFailable.ToOption();

         Assert.That( option.IsSome, Is.True );
         Assert.That( option.OrThrow, Is.EqualTo( "hubba" ) );
      }

      [Test]
      [Description( "Failure wird zu Failable.Failure konvertiert" )]
      public void ConvertFailable_Failure()
      {
         var EFailable = EFailable<string>.Failure( new ArgumentException( "abc" ) );
         Failable<string, Exception> failable = EFailable.ToFailable();

         Assert.That( failable.IsFailure, Is.True );
      }

      [Test]
      [Description( "Success wird zu Failable.Success konvertiert" )]
      public void ConvertToFailable_Success()
      {
         var EFailable = EFailable<string>.Success( "hubba" );
         Failable<string, Exception> failable = EFailable.ToFailable();

         Assert.That( failable.IsSuccess, Is.True );
      }

      [Test]
      [Description( "Failure wird zu EOption.Failure konvertiert" )]
      public void ConvertToEOption_Failure()
      {
         var EFailable = EFailable<string>.Failure( new ArgumentException( "abc" ) );
         EOption<string> EOption = EFailable.ToEOption();

         Assert.That( EOption.IsFailure, Is.True );
      }

      [Test]
      [Description( "Success wird zu EOption.Some konvertiert" )]
      public void ConvertToEOption_Success()
      {
         var EFailable = EFailable<string>.Success( "hubba" );
         EOption<string> EOption = EFailable.ToEOption();

         Assert.That( EOption.IsSome, Is.True );
      }
      #endregion Convert
   }
}