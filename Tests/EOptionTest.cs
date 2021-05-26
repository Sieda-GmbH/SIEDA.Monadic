using System;
using SIEDA.MonadicTests.HelperClass;
using NUnit.Framework;
using SIEDA.Monadic;

namespace SIEDA.MonadicTests
{
   [TestFixture]
   [Description( "Prüft Verhalten von SIEDA's EOption<T,E>." )]
   public class EOptionTest
   {
      #region Construction

      [Test]
      public void ConstructNone_ViaSomeIsNull()
      {
         var testValue = EOption<object>.From( null );

         Assert.That( testValue.IsSome, Is.False );
         Assert.That( testValue.IsNone, Is.True );
         Assert.That( testValue.IsFailure, Is.False );
      }

      [Test]
      public void ConstructNone_ViaNullableWithoutValue()
      {
         var testValue = EOption<int>.From( new int?() );

         Assert.That( testValue.IsNone, Is.True );
         Assert.That( testValue.IsSome, Is.False );
         Assert.That( testValue.IsFailure, Is.False );
      }

      [Test]
      public void ExplicitNullThrowsException()
      {
         Assert.Throws<OptionSomeConstructionException>( () => EOption<string>.Some( null ) );
      }

      [Test]
      public void ExplicitNullableThrowsException()
      {
         Assert.Throws<OptionSomeConstructionException>( () => EOption<int>.Some( new int?() ) );
      }

      [Test]
      public void ConstructSome_ViaActualValue()
      {
         var testValue = EOption<object>.Some( new object() );

         Assert.That( testValue.IsSome, Is.True );
         Assert.That( testValue.IsNone, Is.False );
         Assert.That( testValue.IsFailure, Is.False );
      }

      [Test]
      public void ConstructSome_ViaActualNullableValue()
      {
         int? testObject = 1;
         var testValue = EOption<int>.Some( testObject );

         Assert.That( testValue.IsSome, Is.True );
         Assert.That( testValue.IsNone, Is.False );
         Assert.That( testValue.IsFailure, Is.False );
         Assert.That( testValue.Or( 2 ), Is.EqualTo( 1 ) );
      }

      [Test]
      public void ConstructFailure_ErrorNull()
      {
         Assert.Throws<OptionFailureConstructionException>( () => EOption<object>.Failure( null ) );
      }

      [Test]
      public void ConstructFailure_ErrorNotNull()
      {
         var testValue = EOption<object>.Failure( new ArgumentException() );

         Assert.That( testValue.IsSome, Is.False );
         Assert.That( testValue.IsNone, Is.False );
         Assert.That( testValue.IsFailure, Is.True );
      }

      [Test]
      [Description( "Kann erfolgreiche EOptions vom Wertetyp 'Exception' konstruieren." )]
      public void ConstructSome_ValueIsAnException()
      {
         //WICHTIG: Es gibt Argumente, das hier zu verbieten und analog zum EOption auf ein Failure zu mappen
         //         Es gibt aber auch Argumente, es so zu lassen wie es ist. Aktuell wurde sich für die
         //         weniger invasive Variante entschieden, vor allem da es weniger implizite Sachen macht.
         //         Dieser Test dient zur Dokumentation dieses Verhaltens.
         var x = EOption<Exception>.Some( new Exception() );

         Assert.That( x.IsSome, Is.True );
         Assert.That( x.IsNone, Is.False );
         Assert.That( x.IsFailure, Is.False );
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
      public void ToString_Some()
      {
         Assert.That( EOption<TestObj>.From( new TestObj( "hallo" ) ).ToString(), Is.EqualTo( "[EOption<TestObj>.Some: Object 'hallo']" ) );
      }

      [Test]
      public void ToString_None()
      {
         Assert.That( EOption<TestObj>.From( null ).ToString(), Is.EqualTo( "[EOption<TestObj>.None]" ) );
      }

      [Test]
      public void ToString_Failure()
      {
         Assert.That( EOption<TestObj>.Failure( new ArgumentException( "evil" ) ).ToString(), Is.EqualTo( "[EOption<TestObj>.Failure: System.ArgumentException: evil]" ) );
      }

      #endregion ToString

      #region Equals

      [Test]
      [Description( "Beide haben denselben Wert." )]
      public void Equals_SomeEqual()
      {
         var t1 = new Tuple<string, int>( "abc", 123 );
         var t2 = new Tuple<string, int>( "abc", 123 );

         var x = EOption<Tuple<string, int>>.Some( t1 );
         var y = EOption<Tuple<string, int>>.Some( t2 );

         Assert.That( t1.Equals( t2 ), Is.True );
         Assert.That( x.Equals( y ), Is.True );
      }

      [Test]
      [Description( "Beide haben Werte, aber unterschiedlich." )]
      public void Equals_SomeInequal()
      {
         var x = EOption<int>.Some( 4 );
         var y = EOption<int>.Some( 5 );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Beide haben denselben Fehler, aber es ist nicht die gleiche Instanz!" )]
      public void Equals_FailedEqualObjects()
      {
         var x = EOption<int>.Failure( new ArgumentException() );
         var y = EOption<int>.Failure( new ArgumentException() );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Beide haben eine gemeinsame Fehlerinstanz!" )]
      public void Equals_FailedSameInstance()
      {
         var e = new ArgumentException();
         var x = EOption<int>.Failure( e );
         var y = EOption<int>.Failure( e );

         Assert.That( x.Equals( y ), Is.True );
      }

      [Test]
      [Description( "Gescheitertes EOption ist ungleich jedem Erfolgreichen." )]
      public void Equals_FailedInequalToSome()
      {
         var x = EOption<int>.Failure( new ArgumentException( "how appaling!" ) );
         var y = EOption<int>.Some( 0 );
         var z = EOption<EOption<int>>.Some( y );

         Assert.That( x.Equals( y ), Is.False );
         Assert.That( x.Equals( z ), Is.False );
      }

      [Test]
      [Description( "Leeres EOption ist nur gleich andere leeren EOptions." )]
      public void Equals_None()
      {
         var some = EOption<string>.Some( "something" );
         var fail = EOption<string>.Failure( new ArgumentException( "inexcusable" ) );

         Assert.That( EOption<string>.None.Equals( some ), Is.False );
         Assert.That( EOption<string>.None.Equals( fail ), Is.False );
         Assert.That( EOption<string>.None.Equals( EOption<string>.None ), Is.True );
      }

      [Test]
      [Description( "EOptions zu unterschiedlichen Typen sind unterschiedlich." )]
      public void Equals_DifferentTypeInequal()
      {
         var t1 = new Tuple<string, int>( "abc", 123 );
         var t2 = new Tuple<string, string>( "abc", "123" );

         var x = EOption<Tuple<string, int>>.Some( t1 );
         var y = EOption<Tuple<string, string>>.Some( t2 );

         Assert.That( t1.Equals( t2 ), Is.False );
         Assert.That( x.Equals( y ), Is.False );
      }
      

      [Test]
      [Description( "EOptions zu unterschiedlichen Typen sind unterschiedlich - für Failure wird keine Ausnahme gemacht." )]
      public void Equals_DifferentTypeInequalForFailure()
      {
         var x = EOption<int>.Failure( new ArgumentException() );
         var y = EOption<object>.Failure( new ArgumentException() );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "EOptions zu unterschiedlichen Typen sind unterschiedlich - für None wird keine Ausnahme gemacht." )]
      public void Equals_DifferentTypeInequalForNone()
      {
         var x = EOption<int>.None;
         var y = EOption<object>.None;

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "EOptions sind equivalent zu Options mit Right-Hand-Side Exception" )]
      public void Equals_Option()
      {
         var aEOption = EOption<int>.Some( 4 );
         var aOption = Option<int, Exception>.Some( 4 );

         var bEOption = EOption<int>.None;
         var bOption = Option<int, Exception>.None;

         var exception = new ArgumentException();
         var cEOption = EFailable<int>.Failure( exception );
         var cOption = Failable<int, Exception>.Failure( exception );

         Assert.That( aEOption.GetHashCode(), Is.EqualTo( aOption.GetHashCode() ), "HashCode not correct (Success-Case)" );
         Assert.That( bEOption.GetHashCode(), Is.EqualTo( bOption.GetHashCode() ), "HashCode not correct (None-Case)" );
         Assert.That( cEOption.GetHashCode(), Is.EqualTo( cOption.GetHashCode() ), "HashCode not correct (Failure-Case)" );

         Assert.That( aEOption, Is.EqualTo( aOption ), "EOption-Equals is buggy! (Some-Case)" );
         Assert.That( bEOption, Is.EqualTo( bOption ), "EOption-Equals is buggy! (None-Case)" );
         Assert.That( cEOption, Is.EqualTo( cOption ), "EOption-Equals is buggy! (Failure-Case)" );
         Assert.That( aOption, Is.EqualTo( aEOption ), "Implementation of Option is not accepting EOption! (Some-Case)" );
         Assert.That( bOption, Is.EqualTo( bEOption ), "Implementation of Option is not accepting EOption! (None-Case)" );
         Assert.That( cOption, Is.EqualTo( cEOption ), "Implementation of Option is not accepting EOption! (Failure-Case)" );

         Assert.That( aEOption, Is.Not.EqualTo( bOption ) ); //sanity-checks
         Assert.That( cOption, Is.Not.EqualTo( bEOption ) );
      }

      [Test]
      [Description( "EOptions sind nie equivalent zu Options ohne Right-Hand-Side Exception" )]
      public void Equals_Option_DifferentType()
      {
         var aEOption = EOption<int>.Some( 4 );
         var aOption = Option<int, string>.Some( 4 );

         var bEOption = EOption<int>.None;
         var bOption = Option<int, string>.None;

         var exception = new ArgumentException();
         var cEOption = EFailable<int>.Failure( exception );
         var cOption = Failable<int, string>.Failure( "whatever" );

         Assert.That( aEOption, Is.Not.EqualTo( aOption ) );
         Assert.That( bEOption, Is.Not.EqualTo( bOption ) );
         Assert.That( cEOption, Is.Not.EqualTo( cOption ) );
         Assert.That( aOption, Is.Not.EqualTo( aEOption ) );
         Assert.That( bOption, Is.Not.EqualTo( bEOption ) );
         Assert.That( cOption, Is.Not.EqualTo( cEOption ) );
      }
      #endregion Equals

      #region Accessing Value

      [Test]
      public void Or_FailureOrNull()
      {
         Assert.That( EOption<object>.Failure( new ArgumentException() ).Or( null ), Is.Null );
      }

      [Test]
      public void Or_FailureOrValue()
      {
         Assert.That( EOption<int>.Failure( new ArgumentException() ).Or( 1 ), Is.EqualTo( 1 ) );
      }

      [Test]
      public void Or_Value()
      {
         Assert.That( EOption<int>.Some( 1 ).Or( 2 ), Is.EqualTo( 1 ) );
      }

      [Test]
      public void Error_ThrowsIfSome()
      {
         Assert.Throws<OptionNoFailureException>( () => EOption<string>.Some( "HAPPY" ).FailureOrThrow() );
      }

       [Test]
      public void Error_ReturnsFailure()
      {
         var argEx = new ArgumentException();
         Assert.That( EOption<string>.Failure( argEx ).FailureOrThrow(), Is.EqualTo( argEx ) );
      }

      [Test]
      public void OrThrow_ThrowsIfFailure()
      {
         Assert.Throws<OptionFailureException>( () => EOption<string>.Failure( new ArgumentException() ).OrThrow() );
      }
      

      [Test]
      public void OrThrow_DoesNotThrowIfSome()
      {
         var testValue = ""; //to be overwritten by "happy"

         Assert.DoesNotThrow( () => testValue = EOption<string>.Some( "HAPPY" ).OrThrow() );
         Assert.That( testValue, Is.EqualTo( "HAPPY" ) );
      }

      [Test]
      public void OrThrowWithText_DoesThrowIfFailure()
      {
         Assert.Throws<OptionFailureException>( () => EOption<string>.Failure( new ArgumentException() ).OrThrow( "Test" ) );
      }

      [Test]
      public void OrThrowWithText_DoesNotThrowIfSome()
      {
         var testValue = 0;

         Assert.DoesNotThrow( () => testValue = EOption<int>.Some( 1 ).OrThrow( "Test" ) );
         Assert.That( testValue, Is.EqualTo( 1 ) );
      }

      #endregion Accessing Value

      #region Subtypes

      [Test]
      public void OrWithSubtype_Some()
      {
         var myClass = new MyClass();
         var mySubclass = new MySubclass();

         var underTest = EOption<MyClass>.Some( myClass );
         var actualValue = underTest.Or( mySubclass );

         Assert.That( actualValue, Is.SameAs( myClass ) );
      }

      [Test]
      public void OrWithSubtype_None()
      {
         var mySubclass = new MySubclass();

         var underTest = EOption<MyClass>.None;
         var actualValue = underTest.Or( mySubclass );

         Assert.That( actualValue, Is.SameAs( mySubclass ) );
      }

      [Test]
      public void OrWithSubtype_Failure()
      {
         var mySubclass = new MySubclass();

         var underTest = EOption<MyClass>.Failure( new ArgumentException( "irrelevant" ) );
         var actualValue = underTest.Or( mySubclass );

         Assert.That( actualValue, Is.SameAs( mySubclass ) );
      }

      [Test]
      public void OrWithParallelSubtype_Some()
      {
         var mySubclass = new MySubclass();
         var myOtherSubclass = new MyOtherSubclass();

         var underTest = EOption<MyClass>.Some( mySubclass );
         var actualValue = underTest.Or( myOtherSubclass );

         Assert.That( actualValue, Is.SameAs( mySubclass ) );
      }

      [Test]
      public void OrWithParallelSubtype_None()
      {
         var myOtherSubclass = new MyOtherSubclass();

         var underTest = EOption<MyClass>.None;
         var actualValue = underTest.Or( myOtherSubclass );

         Assert.That( actualValue, Is.SameAs( myOtherSubclass ) );
      }

      [Test]
      public void OrWithParallelSubtype_Failure()
      {
         var myOtherSubclass = new MyOtherSubclass();

         var underTest = EOption<MyClass>.Failure( new ArgumentException( "irrelevant" ) );
         var actualValue = underTest.Or( myOtherSubclass );

         Assert.That( actualValue, Is.SameAs( myOtherSubclass ) );
      }


      #endregion Subtypes

      #region Map and Nesting
      [Test]
      [Description( "Die Map-Operation wird auf Erfolge angewendet." )]
      public void Map_Success()
      {
         var original = EOption<string>.Some( "hallo" );
         var result = original.Map( s => s+= " welt" );
         Assert.That( result.OrThrow, Is.EqualTo( "hallo welt" ) );
      }

      [Test]
      [Description( "Die Map-Operation wird nicht auf Leerwerte angewendet." )]
      public void Map_None()
      {
         var original = EOption<string>.None;
         var result = original.Map( s => s + "!!!" );
         Assert.That( result.IsNone, Is.True );
      }

      [Test]
      [Description( "Die Map-Operation wird nicht auf Fehlschläge angewendet." )]
      public void Map_Failure()
      {
         var argEx = new ArgumentException();

         var original = EOption<string>.Failure( argEx );
         var result = original.Map( s => s+= " welt" );

         Assert.That( result.FailureOrThrow, Is.EqualTo( argEx ) );
      }


      [Test]
      [Description( "Die Map-Operation ist kompatibel mit Verschachtelung" )]
      public void Map_NestingInMap()
      {
         var flag1 = EOption<bool>.Some( true );

         var result = flag1.Map( _ => EOption<bool>.Some( true ) );

         Assert.That( result.Or( EOption<bool>.Failure( new ArgumentException( "disgusting" ) ) ).Or( false ), Is.True );
      }

      [Test]
      [Description( "Verschachtelte Fallunterscheidungen mit FlatMap." )]
      public void Map_NestingInFlatMap()
      {
         var flag1 = EOption<bool>.Some( true );
         var flag2 = EOption<bool>.Some( true );

         var result = flag1.FlatMap( outerBoolVal => flag2.Map( boolVal => boolVal && outerBoolVal ? 3 : 2 ) );

         Assert.That( result.Or( -999 ), Is.EqualTo( 3 ) );
      }

      [Test]
      [Description( "Verschachtelte Fallunterscheidungen mit Wertübergabe mit FlatMap." )]
      public void ValuePropagationInFlatMap()
      {
         var hallo = EOption<string>.Some( "hallo" );
         var sp = EOption<string>.Some( " " );

         var result = hallo.FlatMap( h => sp.Map( space => h + space + "welt" ) );

         Assert.That( result.Or( "nix da" ), Is.EqualTo( "hallo welt" ) );
      }

      [Test]
      [Description( "Map hat keine Probleme mit Typveränderung, weder zur Lauf- noch zur Compilezeit." )]
      public void MapToDifferentType()
      {
         var one = EOption<int>.Some( 1 );
         EOption<string> onePlusOne = one.Map( i => $"{i}+1=2" );

         Assert.That( onePlusOne.OrThrow(), Is.EqualTo( "1+1=2" ) );
      }
      #endregion Map and Nesting

      #region Is

      [Test]
      [Description( "Check mittels Direktvergleich - Vergleich liefert 'true'" )]
      public void ContainsDefinedTrue()
      {
         var f = EOption<string>.Some( "hubba" );

         Assert.That( f.Is( "hubba" ), Is.EqualTo( true ) );
      }

      [Test]
      [Description( "Check mittels Direktvergleich - Vergleich liefert 'false'" )]
      public void ContainsDefinedFalse()
      {
         var f = EOption<string>.Some( "hubba" );

         Assert.That( f.Is( "whatever" ), Is.EqualTo( false ) );
      }

      [Test]
      [Description( "Check mittels Direktvergleich - None immer 'false'" )]
      public void ContainsFalseWhenNone()
      {
         var f = EOption<string>.Failure( new ArgumentException() );

         Assert.That( f.Is( "hubba" ), Is.EqualTo( false ) );
      }

      #endregion Is

      #region Holds

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich liefert 'true'" )]
      public void Is_PredicateMatchingDefined()
      {
         var EOption = EOption<string>.Some( "hubba" );

         Assert.That( EOption.Holds( s => s.Equals( "hubba" ) ), Is.EqualTo( true ) );
                                                   
      }

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich liefert 'false'" )]
      public void Is_PredicateNonMatchingDefined()
      {
         var EOption = EOption<string>.Some( "hubba" );

         Assert.That( EOption.Holds( s => s.Equals( "hubba-hub" ) ), Is.EqualTo( false ) );
                                                   
      }

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich findet nicht statt" )]
      public void Is_PredicateUndefined()
      {
         var EOption = EOption<string>.Failure( new ArgumentException() );

         Assert.That( EOption.Holds( s => s.Equals( "hubba-hub" ) ), Is.EqualTo( false ) );
                                                   
      }

      #endregion Holds

      #region TryGet

      [Test]
      [Description( "TryGetValue() produziert korrektes Boolean-Result bei Failure" )]
      public void TryGet_Result_Failure()
      {
         var EOption = EOption<string>.Failure( new ArgumentException() );

         Assert.IsFalse( EOption.TryGetValue( out var s ) );
      }

      [Test]
      [Description( "TryGetValue() schreibt keinen Wert bei Failure" )]
      public void TryGet_Value_Failure()
      {
         var EOption = EOption<string>.Failure( new ArgumentException() );
         EOption.TryGetValue( out var s );

         Assert.IsNull( s );
      }

      [Test]
      [Description( "TryGetValue() produziert korrektes Boolean-Result bei Some" )]
      public void TryGet_Value_Some()
      {
         var EOption = EOption<string>.Some( "blub" );

         Assert.IsTrue( EOption.TryGetValue( out var s ) );
      }

      [Test]
      [Description( "TryGetValue() schreibt keinen Wert bei Some" )]
      public void TryGet_Result_Some()
      {
         var EOption = EOption<string>.Some( "blub" );
         EOption.TryGetValue( out var s );

         Assert.That( s, Is.EqualTo( "blub" ) );
      }

      [Test]
      [Description( "TryGetValue() produziert korrektes Boolean-Result bei None" )]
      public void TryGet_Value_None()
      {
         var EOption = EOption<string>.None;

         Assert.IsFalse( EOption.TryGetValue( out var s ) );
      }

      [Test]
      [Description( "TryGetValue() schreibt keinen Wert bei None" )]
      public void TryGet_Result_None()
      {
         var EOption = EOption<string>.None;
         EOption.TryGetValue( out var s );

         Assert.IsNull( s );
      }

      #endregion TryGet

      #region Convert
      
      [Test]
      [Description( "Some wird zu Maybe.Some konvertiert" )]
      public void ConvertToMaybe_Some()
      {
         var EOption = EOption<string>.Some( "hallo" );
         var maybe = EOption.ToMaybe();

         Assert.That( maybe.IsSome, Is.True );
         Assert.That( maybe.OrThrow, Is.EqualTo( "hallo" ) );
      }

      [Test]
      [Description( "Failure wird zu Maybe.None konvertiert" )]
      public void ConvertToMaybe_Failure()
      {
         var EOption = EOption<string>.Failure( new ArgumentException() );
         var maybe = EOption.ToMaybe();

         Assert.That( maybe.IsNone, Is.True );
      }

      [Test]
      [Description( "None wird zu Maybe.None konvertiert" )]
      public void ConvertToMaybe_None()
      {
         var EOption = EOption<string>.None;
         var maybe = EOption.ToMaybe();

         Assert.That( maybe.IsNone, Is.True );
      }

      [Test]
      [Description( "Some wird zu Failable.Some konvertiert" )]
      public void ConvertToFailable_Some()
      {
         var EOption = EOption<string>.Some( "hallo" );
         var failable = EOption.ToFailable( new ArgumentException() );

         Assert.That( failable.IsSuccess, Is.True );
         Assert.That( failable.OrThrow, Is.EqualTo( "hallo" ) );
      }

      [Test]
      [Description( "Failure wird zu Failable.Failure konvertiert" )]
      public void ConvertToFailable_Failure()
      {
         var EOption = EOption<string>.Failure( new ArgumentException("msg") );
         var failable = EOption.ToFailable( new ArgumentException("notMsg") ); //different exception text!

         Assert.That( failable.IsFailure, Is.True );
         Assert.That( failable.FailureOrThrow().Message, Is.EqualTo("msg") );
      }

      [Test]
      [Description( "None wird zu Failable.Failure konvertiert" )]
      public void ConvertToFailable_None()
      {
         var EOption = EOption<string>.None;
         var failable = EOption.ToFailable( new ArgumentException("msg") );

         Assert.That( failable.IsFailure, Is.True );
         Assert.That( failable.FailureOrThrow().Message, Is.EqualTo("msg") );
      }

      [Test]
      [Description( "Some wird zu EFailable.Some konvertiert" )]
      public void ConvertToEFailable_Some()
      {
         var EOption = EOption<string>.Some( "hallo" );
         var failable = EOption.ToEFailable( new ArgumentException() );

         Assert.That( failable.IsSuccess, Is.True );
         Assert.That( failable.OrThrow, Is.EqualTo( "hallo" ) );
      }

      [Test]
      [Description( "Failure wird zu EFailable.Failure konvertiert" )]
      public void ConvertToEFailable_Failure()
      {
         var EOption = EOption<string>.Failure( new ArgumentException( "msg" ) );
         EFailable<string> failable = EOption.ToEFailable( new ArgumentException( "to-be-used-on-none!" ) );

         Assert.That( failable.IsFailure, Is.True );
         Assert.That( failable.FailureOrThrow().Message, Is.EqualTo( "msg" ) );
      }

      [Test]
      [Description( "None wird zu EFailable.Failure konvertiert" )]
      public void ConvertToEFailable_None()
      {
         var EOption = EOption<string>.None;
         var failable = EOption.ToEFailable( new ArgumentException( "msg" ) );

         Assert.That( failable.IsFailure, Is.True );
         Assert.That( failable.FailureOrThrow().Message, Is.EqualTo( "msg" ) );
      }

      [Test]
      [Description( "Some wird zu Option.Some konvertiert" )]
      public void ConvertToOption_Some()
      {
         var EOption = EOption<string>.Some( "hallo" );
         var option = EOption.ToOption();

         Assert.That( option.IsSome, Is.True );
         Assert.That( option.OrThrow, Is.EqualTo( "hallo" ) );
      }

      [Test]
      [Description( "Failure wird zu Option.Failure konvertiert" )]
      public void ConvertToOption_Failure()
      {
         var EOption = EOption<string>.Failure( new ArgumentException( "msg" ) );
         Option<string, Exception> option = EOption.ToOption();

         Assert.That( option.IsFailure, Is.True );
         Assert.That( option.FailureOrThrow().Message, Is.EqualTo( "msg" ) );
      }

      [Test]
      [Description( "None wird zu Option.None konvertiert" )]
      public void ConvertToOption_None()
      {
         var EOption = EOption<string>.None;
         var option = EOption.ToOption();

         Assert.That( option.IsNone, Is.True );
      }
      #endregion Convert
   }
}