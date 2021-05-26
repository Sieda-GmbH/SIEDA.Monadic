using System;
using SIEDA.MonadicTests.HelperClass;
using NUnit.Framework;
using SIEDA.Monadic;

namespace SIEDA.MonadicTests
{
   [TestFixture]
   [Description( "Prüft Verhalten von SIEDA's Maybe<T>." )]
   public class MaybeTest
   {
      #region Construction

      [Test]
      [Description( "Maybe mit Wert ist nicht \"leer\"" )]
      public void DefinedIsSome()
      {
         var x = Maybe<int>.Some( 0 );
         var y = Maybe<int>.From( 0 );

         Assert.That( x.IsSome, Is.True );
         Assert.That( x.IsNone, Is.False );
         Assert.AreEqual( x, y );
      }

      [Test]
      [Description( "Maybe mit Wert 'null' kann kein Some sein." )]
      public void ExplicitNullThrowsException()
      {
         Assert.Throws<MaybeSomeConstructionException>( () => Maybe<object>.Some( null ) );
      }

      [Test]
      [Description( "Maybe mit Wert Nullable kann kein Some sein." )]
      public void ExplicitNullableThrowsException()
      {
         var x = Maybe<int>.From( new int?() );

         Assert.That( x.IsSome, Is.False );
         Assert.That( x.IsNone, Is.True );
      }

      [Test]
      [Description( "Maybe mit Wert 'null' produziert \"leer\"" )]
      public void ExplicitNullTreatedLikeNone()
      {
         var x = Maybe<object>.From( null );

         Assert.That( x.IsSome, Is.False );
         Assert.That( x.IsNone, Is.True );
      }

      [Test]
      [Description( "Maybe mit Wert Nullable ohne Wert produziert \"leer\"" )]
      public void ExplicitNullableTreatedLikeNone()
      {
         var x = Maybe<int>.From( new int?() );

         Assert.That( x.IsSome, Is.False );
         Assert.That( x.IsNone, Is.True );
      }

      [Test]
      [Description( "None-Maybe ist \"leer\"" )]
      public void NoneIsNone()
      {
         var x = Maybe<int>.None;

         Assert.That( x.IsSome, Is.False );
         Assert.That( x.IsNone, Is.True );
      }

      [Test]
      public void NullableIsSome()
      {
         int? testObject = 5;
         var testValue = Maybe<int>.Some( testObject );

         Assert.That( testValue.IsSome, Is.True );
         Assert.That( testValue.Or( 0 ), Is.EqualTo( testObject.Value ) );
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
         Assert.That( Maybe<TestObj>.From( new TestObj( "hallo" ) ).ToString(), Is.EqualTo( "[Maybe<TestObj>.Some: Object 'hallo']" ) );
      }

      [Test]
      public void ToString_None()
      {
         Assert.That( Maybe<TestObj>.From( null ).ToString(), Is.EqualTo( "[Maybe<TestObj>.None]" ) );
      }

      #endregion ToString

      #region Equals

      [Test]
      [Description( "Beide haben denselben Wert." )]
      public void ValueEqual()
      {
         var t1 = new Tuple<string, int>( "abc", 123 );
         var t2 = new Tuple<string, int>( "abc", 123 );

         var x = Maybe<Tuple<string, int>>.Some( t1 );
         var y = Maybe<Tuple<string, int>>.Some( t2 );

         Assert.That( t1.Equals( t2 ), Is.True );
         Assert.That( x.Equals( y ), Is.True );
      }

      [Test]
      [Description( "Beide haben Werte, aber unterschiedlich." )]
      public void ValueInequal()
      {
         var x = Maybe<int>.Some( 4 );
         var y = Maybe<int>.Some( 5 );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Es ist egal, ob der Nullwert explizit konstruiert wurde oder nicht." )]
      public void NoneEqual()
      {
         var x = Maybe<object>.None;
         var y = Maybe<object>.From( null );

         Assert.That( x.Equals( y ), Is.True );
      }

      [Test]
      [Description( "Undefinierter Wert ist ungleich jedem anderen Wert." )]
      public void NoneInequal()
      {
         var x = Maybe<int>.None;
         var y = Maybe<int>.Some( 0 );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Maybes zu unterschiedlichen Typen sind unterschiedlich." )]
      public void DifferentTypeInequal()
      {
         var t1 = new Tuple<string, int>( "abc", 123 );
         var t2 = new Tuple<string, string>( "abc", "123" );

         var x = Maybe<Tuple<string, int>>.Some( t1 );
         var y = Maybe<Tuple<string, string>>.Some( t2 );

         Assert.That( t1.Equals( t2 ), Is.False );
         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Maybes zu unterschiedlichen Typen sind unterschiedlich - für None wird keine Ausnahme gemacht." )]
      public void DifferentTypeInequalForNone()
      {
         var x = Maybe<int>.None;
         var y = Maybe<object>.None;

         Assert.That( x.Equals( y ), Is.False );
      }

      #endregion Equals

      #region Accessing Value

      [Test]
      public void NoneOrNull()
      {
         Assert.That( Maybe<object>.None.Or( null ), Is.Null );
      }

      [Test]
      public void NoneOrValue()
      {
         Assert.That( Maybe<int>.None.Or( 1 ), Is.EqualTo( 1 ) );
      }

      [Test]
      public void OrValue()
      {
         Assert.That( Maybe<int>.Some( 1 ).Or( 2 ), Is.EqualTo( 1 ) );
      }

      [Test]
      public void NoneOrThrowThrows()
      {
         Assert.Throws<Exception>( () => Maybe<int>.None.OrThrow( new Exception() ) );
      }

      [Test]
      public void OrThrowDoesNotThrow()
      {
         var testValue = 0;

         Assert.DoesNotThrow( () => testValue = Maybe<int>.Some( 1 ).OrThrow( new Exception() ) );
         Assert.That( testValue, Is.EqualTo( 1 ) );
      }

      #endregion Accessing Value

      #region Subtypes

      [Test]
      public void OrWithDirectSubtype_Some()
      {
         var myClass = new MyClass();
         var mySubclass = new MySubclass();

         var underTest = Maybe<MyClass>.Some( myClass );
         var actualValue = underTest.Or( mySubclass );

         Assert.That( actualValue, Is.SameAs( myClass ) );
      }

      [Test]
      public void OrWithDirectSubtype_None()
      {
         var mySubclass = new MySubclass();

         var underTest = Maybe<MyClass>.None;
         var actualValue = underTest.Or( mySubclass );

         Assert.That( actualValue, Is.SameAs( mySubclass ) );
      }

      [Test]
      public void OrWithParallelSubtype_Some()
      {
         var mySubclass = new MySubclass();
         var myOtherSubclass = new MyOtherSubclass();

         var underTest = Maybe<MyClass>.Some( mySubclass );
         var actualValue = underTest.Or( myOtherSubclass );

         Assert.That( actualValue, Is.SameAs( mySubclass ) );
      }

      [Test]
      public void OrWithParallelSubtype_None()
      {
         var myOtherSubclass = new MyOtherSubclass();

         var underTest = Maybe<MyClass>.None;
         var actualValue = underTest.Or( myOtherSubclass );

         Assert.That( actualValue, Is.SameAs( myOtherSubclass ) );
      }

      #endregion Subtypes

      #region Mapping

      [Test]
      public void NoneMap()
      {
         var testValue = Maybe<int>.Some( 1 );
         Func<string, int> testFunc = _ => throw new Exception();

         Assert.DoesNotThrow( () => testValue = Maybe<string>.None.Map( testFunc ) );
         Assert.That( testValue.IsSome, Is.False );
      }

      public void NoneFlatMap()
      {
         var testValue = Maybe<int>.Some( 1 );
         Func<string, Maybe<int>> testFunc = _ => throw new Exception();

         Assert.DoesNotThrow( () => testValue = Maybe<string>.None.FlatMap( testFunc ) );
         Assert.That( testValue.IsSome, Is.False );
      }

      /// <summary>
      /// Man kann natürlich auch sagen, <see cref="MappingNullTreatedLikeNone"/> könnte genauso gut durch <see cref="FlatMappingNone"/> dargestellt werden.
      ///
      /// Nur ist die Frage, wie man hier <see langword="null"/> als Rückgabewert interpretieren will: hier ist es <see cref="Maybe{T}.None"/>, weil das
      /// konsistent zu <see cref="Maybe{TValue}.From(TValue)"/> ist.
      /// </summary>
      [Test]
      [Description( "In Map wird Null als None interpretiert." )]
      public void MappingNullTreatedLikeNone()
      {
         var x = Maybe<int>.Some( 5 ).Map( _ => (object) null );

         Assert.That( x.IsSome, Is.False );
         Assert.That( x.IsNone, Is.True );
      }

      [Test]
      [Description( "Um Ergebnis None zu erhalten kann man auch als Map-Ergebnis ein Maybe haben." )]
      public void FlatMappingNone()
      {
         var x = Maybe<int>.Some( 5 ).FlatMap( _ => Maybe<object>.None );

         Assert.That( x.IsSome, Is.False );
         Assert.That( x.IsNone, Is.True );
      }

      [Test]
      [Description( "Die Map-Operation ist kompatibel mit Verschachtelung" )]
      public void NestingInMap()
      {
         var flag1 = Maybe<bool>.Some( false )
            .Map( _ => Maybe<bool>.Some( true ) )
            .Or( Maybe<bool>.None )
            .Or( false );

         Assert.That( flag1, Is.True );
      }

      [Test]
      [Description( "Verschachtelte Fallunterscheidungen mit FlatMap." )]
      public void NestingInFlatMap()
      {
         var flag1 = Maybe<bool>.Some( true );
         var flag2 = Maybe<bool>.Some( true );

         var result = flag1.FlatMap( _ => flag2.Map( boolVal => boolVal ? 3 : 2 ) );

         Assert.That( result.Or( -999 ), Is.EqualTo( 3 ) );
      }

      [Test]
      [Description( "Verschachtelte Fallunterscheidungen mit Wertübergabe mit FlatMap." )]
      public void ValuePropagationInFlatMap()
      {
         var hallo = Maybe<string>.Some( "hallo" );
         var sp = Maybe<string>.Some( " " );

         var result = hallo.FlatMap( h => sp.Map( space => h + space + "welt" ) );

         Assert.That( result.Or( "nix da" ), Is.EqualTo( "hallo welt" ) );
      }

      [Test]
      [Description( "Map hat keine Probleme mit Typveränderung, weder zur Lauf- noch zur Compilezeit." )]
      public void MapToDifferentType()
      {
         var one = Maybe<int>.Some( 1 );
         Maybe<string> onePlusOne = one.Map( i => $"{i}+1=2" );

         Assert.That( onePlusOne.OrThrow(), Is.EqualTo( "1+1=2" ) );
      }

      [Test]
      [Description( "Verschachtelte Fallunterscheidungen." )]
      public void NestingInOrValueComplex()
      {
         var flag1 = Maybe<bool>.None;
         var flag2 = Maybe<bool>.None;
         var flag3 = Maybe<bool>.Some( true );

         int flag2Result = flag2        // Maybe<int>  -> int
            .Map( f2 =>
               flag3              // Maybe<bool> -> int
                  .Map( f3 => f3 ? 3 : 4 )       // Maybe<bool> -> Maybe<int>
                  .Or( 5 ) )       // Maybe<int> -> int
            .Or( 8 );

         int result = flag1
            .Map( f1 => f1 ? 1 : 2 )       // Maybe<bool> -> Maybe<int>
            .Or( flag2Result );

         Assert.That( result, Is.EqualTo( 8 ) );
      }

      #endregion Mapping

      #region Is

      [Test]
      [Description( "Check mittels Direktvergleich - Vergleich liefert 'true'" )]
      public void ContainsDefinedTrue()
      {
         var maybe = Maybe<string>.Some( "hubba" );

         Assert.That( maybe.Is(  "hubba" ), Is.EqualTo( true ) );
      }

      [Test]
      [Description( "Check mittels Direktvergleich - Vergleich liefert 'false'" )]
      public void ContainsDefinedFalse()
      {
         var maybe = Maybe<string>.Some( "hubba" );

         Assert.That( maybe.Is( "whatever" ), Is.EqualTo( false ) );
      }

      [Test]
      [Description( "Check mittels Direktvergleich - None immer 'false'" )]
      public void ContainsFalseWhenNone()
      {
         var maybe = Maybe<string>.None;

         Assert.That( maybe.Is( "hubba" ), Is.EqualTo( false ) );
      }

      #endregion Is

      #region Holds

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich liefert 'true'" )]
      public void PredicateMatchingDefined()
      {
         var maybe = Maybe<string>.Some( "hubba" );

         Assert.That( maybe.Holds( s => s.Equals( "hubba" ) ), Is.EqualTo( true ) );
                                                   
      }

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich liefert 'false'" )]
      public void PredicateNonMatchingDefined()
      {
         var maybe = Maybe<string>.Some( "hubba" );

         Assert.That( maybe.Holds( s => s.Equals( "hubba-hub" ) ), Is.EqualTo( false ) );
                                                   
      }

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich findet nicht statt" )]
      public void PredicateUndefined()
      {
         var maybe = Maybe<string>.None;

         Assert.That( maybe.Holds( s => s.Equals( "hubba-hub" ) ), Is.EqualTo( false ) );
      }

      #endregion Holds

      #region Or

      [Test]
      [Description( "OrThrow() wirft korrekte Exception bei None" )]
      public void OrThrow_None()
      {
         Assert.Throws<MaybeNoneException>( () => Maybe<string>.None.OrThrow() );
      }

      [Test]
      [Description( "OrThrow() liefert den Wert bei Some" )]
      public void OrThrow_Some()
      {
         var maybe = Maybe<string>.Some( "hubba" );
         var testValue = string.Empty;

         Assert.DoesNotThrow( () => testValue = maybe.OrThrow() );
         Assert.That( testValue, Is.EqualTo( "hubba" ) );
      }

      [Test]
      [Description( "OrThrow( exc ) wirf korrekte Exception bei None" )]
      public void OrThrow_E_None()
      {
         var maybe = Maybe<string>.None;

         Assert.Throws<ArgumentNullException>( () => maybe.OrThrow( new ArgumentNullException() ) );
      }

      [Test]
      [Description( "OrThrow( exc ) liefert den Wert bei Some" )]
      public void OrThrow_E_Some()
      {
         var maybe = Maybe<string>.Some( "hubba" );
         var testValue = string.Empty;

         Assert.DoesNotThrow( () => testValue = maybe.OrThrow( new Exception() ) );
         Assert.That( testValue, Is.EqualTo( "hubba" ) );
      }

      #endregion Or

      #region TryGet

      [Test]
      [Description( "TryGet() produziert korrektes Boolean-Result bei None" )]
      public void TryGet_Result_None()
      {
         var maybe = Maybe<string>.None;

         Assert.IsFalse( maybe.TryGetValue( out var s ) );
      }

      [Test]
      [Description( "TryGet() schreibt keinen Wert bei None" )]
      public void TryGet_Value_None()
      {
         var maybe = Maybe<string>.None;
         maybe.TryGetValue( out var s );

         Assert.IsNull( s );
      }

      [Test]
      [Description( "TryGet() produziert korrektes Boolean-Result bei Some" )]
      public void TryGet_Result_Some()
      {
         var maybe = Maybe<string>.Some( "blub" );

         Assert.IsTrue( maybe.TryGetValue( out var s ) );
      }

      [Test]
      [Description( "TryGet() schreibt keinen Wert bei Some" )]
      public void TryGet_Value_Some()
      {
         var maybe = Maybe<string>.Some( "blub" );
         maybe.TryGetValue( out var s );

         Assert.That( s, Is.EqualTo( "blub" ) );
      }

      #endregion TryGet

      #region Flatten
      [Test]
      [Description( "Flattening - Eliminierung redundanter Schachtelungen" )]
      public void Flatten()
      {
         var a = Maybe<string>.Some( "a" );
         var b = Maybe<Maybe<string>>.Some( a );

         Assert.That( b, Is.Not.EqualTo( a ) );
         Assert.That( b.Flatten(), Is.EqualTo( a ) );
      }
      #endregion Flatten

      #region Convert

      [Test]
      [Description( "Definiertes Maybe wird zu Failable.Success konvertiert" )]
      public void ConvertToFailable_Some()
      {
         var maybe = Maybe<string>.Some( "hallo" );
         var failable = maybe.ToFailable( new ArgumentException() );

         Assert.That( failable.IsSuccess, Is.True );
         Assert.That( failable.OrThrow, Is.EqualTo( "hallo" ) );
      }

      [Test]
      [Description( "Leeres Maybe wird zu Failable.Failure konvertiert" )]
      public void ConvertToFailable_None()
      {
         var maybe = Maybe<string>.None;
         Failable<string, ArgumentException> failable = maybe.ToFailable( new ArgumentException("msg") );

         Assert.That( failable.IsFailure, Is.True );
         Assert.That( failable.FailureOrThrow().Message, Is.EqualTo("msg") );
      }

      [Test]
      [Description( "Definiertes Maybe wird zu EFailable.Success konvertiert" )]
      public void ConvertToEFailable_Some()
      {
         var maybe = Maybe<string>.Some( "hallo" );
         var failable = maybe.ToEFailable( new ArgumentException() );

         Assert.That( failable.IsSuccess, Is.True );
         Assert.That( failable.OrThrow, Is.EqualTo( "hallo" ) );
      }

      [Test]
      [Description( "Leeres Maybe wird zu EFailable.Failure konvertiert" )]
      public void ConvertToEFailable_None()
      {
         var maybe = Maybe<string>.None;
         EFailable<string> failable = maybe.ToEFailable( new ArgumentException("msg") );

         Assert.That( failable.IsFailure, Is.True );
         Assert.That( failable.FailureOrThrow().Message, Is.EqualTo("msg") );
      }

      [Test]
      [Description( "Leeres Maybe wird zu Option.None konvertiert" )]
      public void ConvertToOption_None()
      {
         var maybe = Maybe<string>.None;
         var option = maybe.ToOption<int>();

         Assert.That( option.IsNone, Is.True );
      }

      [Test]
      [Description( "Definiertes Maybe wird zu Option.Some konvertiert" )]
      public void ConvertToOption_Some()
      {
         var maybe = Maybe<string>.Some("hubba");
         var option = maybe.ToOption<int>();

         Assert.That( option.IsSome, Is.True );
      }

      [Test]
      [Description( "Leeres Maybe wird zu EOption.None konvertiert" )]
      public void ConvertToEOption_None()
      {
         var maybe = Maybe<string>.None;
         EOption<string> EOption = maybe.ToEOption();

         Assert.That( EOption.IsNone, Is.True );
      }

      [Test]
      [Description( "Definiertes Maybe wird zu EOption.Some konvertiert" )]
      public void ConvertToEOption_Some()
      {
         var maybe = Maybe<string>.Some( "hubba" );
         EOption<string> EOption = maybe.ToEOption();

         Assert.That( EOption.IsSome, Is.True );
      }
      #endregion Convert
   }
}