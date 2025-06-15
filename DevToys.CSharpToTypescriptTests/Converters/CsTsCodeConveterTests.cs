namespace DevToys.CSharpToTypescript.Converters.Tests
{
    [TestClass()]
    public class CsTsCodeConveterTests
    {
        internal void ConvertTestCore(string baseCode, string expected, DateType dateType, bool toCamelCase, bool publicOnly, bool addExport)
        {
            var conveter = new CsTsCodeConveter(baseCode, dateType, toCamelCase, publicOnly, addExport);
            var actual = conveter.Convert().Trim();
            Assert.AreEqual(expected.Trim(), actual);
        }

        [TestMethod]
        public void ConvertTestNumeric()
        {
            this.ConvertTestCore("""
                public class NumericTest
                {
                    public int IntValue { get; set; }
                    public long LongValue { get; set; }
                    public float FloatValue { get; set; }
                    public double DoubleValue { get; set; }
                    public decimal DecimalValue { get; set; }
                    public sbyte SByteValue { get; set; }
                    public byte ByteValue { get; set; }
                    public short ShortValue { get; set; }
                    public ushort UShortValue { get; set; }
                    public uint UIntValue { get; set; }
                    public ulong ULongValue { get; set; }
                    public nint NIntValue { get; set; }
                    public nuint NUIntValue { get; set; }
                    public SByte SByteValue2 { get; set; }
                    public Byte ByteValue2 { get; set; }
                    public IntPtr IntPtrValue { get; set; }
                    public UIntPtr UIntPtrValue { get; set; }
                    public Int16 Int16Value { get; set; }
                    public UInt16 UInt16Value { get; set; }
                    public Int32 Int32Value { get; set; }
                    public UInt32 UInt32Value { get; set; }
                    public Int64 Int64Value { get; set; }
                    public UInt64 UInt64Value { get; set; }
                    public Single SingleValue { get; set; }
                    public Double DoubleValue2 { get; set; }
                    public Decimal DecimalValue2 { get; set; }
                }
                """,
                """
                interface NumericTest {
                    intValue: number;
                    longValue: number;
                    floatValue: number;
                    doubleValue: number;
                    decimalValue: number;
                    sByteValue: number;
                    byteValue: number;
                    shortValue: number;
                    uShortValue: number;
                    uIntValue: number;
                    uLongValue: number;
                    nIntValue: number;
                    nUIntValue: number;
                    sByteValue2: number;
                    byteValue2: number;
                    intPtrValue: number;
                    uIntPtrValue: number;
                    int16Value: number;
                    uInt16Value: number;
                    int32Value: number;
                    uInt32Value: number;
                    int64Value: number;
                    uInt64Value: number;
                    singleValue: number;
                    doubleValue2: number;
                    decimalValue2: number;
                }
                """, DateType.Union, true, true, false);
        }

        [TestMethod]
        public void ConvertTestDateToDate()
        {
            this.ConvertTestCore("""
                public class DateTimeTest
                {
                    public DateTime DateTimeValue { get; set; }
                    public DateOnly DateOnlyValue { get; set; }
                    public TimeOnly TimeOnlyValue { get; set; }
                }
                """,
                """
                interface DateTimeTest {
                    dateTimeValue: Date;
                    dateOnlyValue: Date;
                    timeOnlyValue: Date;
                }
                """, DateType.Date, true, true, false);
        }

        [TestMethod]
        public void ConvertTestDateToString()
        {
            this.ConvertTestCore("""
                public class DateTimeTest
                {
                    public DateTime DateTimeValue { get; set; }
                    public DateOnly DateOnlyValue { get; set; }
                    public TimeOnly TimeOnlyValue { get; set; }
                }
                """,
                """
                interface DateTimeTest {
                    dateTimeValue: string;
                    dateOnlyValue: string;
                    timeOnlyValue: string;
                }
                """, DateType.String, true, true, false);
        }

        [TestMethod]
        public void ConvertTestDateUnion()
        {
            this.ConvertTestCore("""
                public class DateTimeTest
                {
                    public DateTime DateTimeValue { get; set; }
                    public DateOnly DateOnlyValue { get; set; }
                    public TimeOnly TimeOnlyValue { get; set; }
                }
                """,
                """
                interface DateTimeTest {
                    dateTimeValue: Date | string;
                    dateOnlyValue: Date | string;
                    timeOnlyValue: Date | string;
                }
                """, DateType.Union, true, true, false);
        }

        [TestMethod]
        public void ConvertTestString()
        {
            this.ConvertTestCore("""
                public class StringTest
                {
                    public string StringValue { get; set; }
                    public char CharValue { get; set; }
                    public String StringValue2 { get; set; }
                    public Char CharValue2 { get; set; }
                }
                """,
                """
                interface StringTest {
                    stringValue: string;
                    charValue: string;
                    stringValue2: string;
                    charValue2: string;
                }
                """, DateType.Union, true, true, false);
        }

        [TestMethod]
        public void ConvertTestArray()
        {
            this.ConvertTestCore("""
                public class Item{
                    public int Id { get; set; }
                    public string Name { get; set; }
                }

                public class ArrayTest
                {
                    public int[] IntArray { get; set; }
                    public string[] StringArray { get; set; }
                    public List<int> IntList { get; set; }
                    public List<string> StringList { get; set; }
                    public IEnumerable<int> IntEnumerable { get; set; }
                    public Array AnyArray { get; set; }
                    public Item[] Items { get; set; }
                    public IEnumerable<Item> ItemEnumerable { get; set; }
                }
                """,
                """
                interface Item {
                    id: number;
                    name: string;
                }

                interface ArrayTest {
                    intArray: number[];
                    stringArray: string[];
                    intList: number[];
                    stringList: string[];
                    intEnumerable: number[];
                    anyArray: any[];
                    items: Item[];
                    itemEnumerable: Item[];
                }
                """, DateType.Union, true, true, false);
        }

        [TestMethod]
        public void ConvertTestDictionary()
        {
            this.ConvertTestCore("""
                public class DictionaryTest
                {
                    public Dictionary<string, int> StringIntDictionary { get; set; }
                    public Dictionary<int, string> IntStringDictionary { get; set; }
                    public IDictionary<string, int> StringIntIDictionary { get; set; }
                    public IReadOnlyDictionary<string, int> StringIntReadOnlyDictionary { get; set; }
                }
                """,
                """
                interface DictionaryTest {
                    stringIntDictionary: {[key: string]: number};
                    intStringDictionary: {[key: number]: string};
                    stringIntIDictionary: {[key: string]: number};
                    stringIntReadOnlyDictionary: {[key: string]: number};
                }
                """, DateType.Union, true, true, false);
        }

        [TestMethod]
        public void ConvertTestTuple()
        {
            this.ConvertTestCore("""
                public class TupleTest
                {
                    public (int, string) SimpleTuple { get; set; }
                    public (int, (int, string?)) NestedTuple { get; set; }
                }
                """,
                """
                interface TupleTest {
                    simpleTuple: [number, string];
                    nestedTuple: [number, [number, string | null]];
                }
                """, DateType.Union, true, true, false);
        }

        [TestMethod]
        public void ConvertTestRecord()
        {
            this.ConvertTestCore("""
                public record RecordTest(int Id, string Name, float Value);
                """,
                """
                interface RecordTest {
                    id: number;
                    name: string;
                    value: number;
                }
                """, DateType.Union, true, true, false);
        }

        [TestMethod]
        public void ConvertTestGeneric()
        {
            this.ConvertTestCore("""
                public interface BaseItem {}
                public interface BaseItem2 {}

                public class GenericTest<T>
                {
                    public T Value { get; set; }
                }

                public class GenericTest2<T1, T2> where T1 : BaseItem, BaseItem2
                {
                    public T1 Value1 { get; set; }
                    public T2 Value2 { get; set; }
                }
                """,
                """
                interface BaseItem {
                }

                interface BaseItem2 {
                }

                interface GenericTest<T> {
                    value: T;
                }

                interface GenericTest2<T1 extends BaseItem & BaseItem2, T2> {
                    value1: T1;
                    value2: T2;
                }
                """, DateType.Union, true, true, false);
        }

        [TestMethod]
        public void ConvertTestInheritance()
        {
            this.ConvertTestCore("""
                public interface BaseInterface
                {
                    public int BaseProperty { get; set; }
                }
                public interface DerivedInterface : BaseInterface
                {
                    public string DerivedProperty { get; set; }
                }
                public interface InheritanceTest : DerivedInterface{}
                """,
                """
                interface BaseInterface {
                    baseProperty: number;
                }

                interface DerivedInterface extends BaseInterface {
                    derivedProperty: string;
                }

                interface InheritanceTest extends DerivedInterface {
                }
                """, DateType.Union, true, true, false);
        }

        [TestMethod]
        public void ConvertTestNullable()
        {
            this.ConvertTestCore("""
                public class NullableTest
                {
                    public int? NullableInt { get; set; }
                    public string? NullableString { get; set; }
                    public DateTime? NullableDateTime { get; set; }
                }
                """,
                """
                interface NullableTest {
                    nullableInt: number | null;
                    nullableString: string | null;
                    nullableDateTime: Date | string | null;
                }
                """, DateType.Union, true, true, false);
        }

        [TestMethod]
        public void ConvertTestCaseTest()
        {
            this.ConvertTestCore("""
                public class CaseTest
                {
                    public int MyProperty { get; set; }
                    public string AnotherProperty { get; set; }
                    public string my_property { get; set; }
                }
                """,
                """
                interface CaseTest {
                    MyProperty: number;
                    AnotherProperty: string;
                    my_property: string;
                }
                """, DateType.Union, false, true, false);
        }

        [TestMethod]
        public void ConvertTestPubicOnly()
        {
            this.ConvertTestCore("""
                public class PublicOnlyTest
                {
                    public int PublicProperty { get; set; }
                    private int PrivateProperty { get; set; }
                    protected int ProtectedProperty { get; set; }
                    internal int InternalProperty { get; set; }
                }
                class PrivateTest
                {
                    public int PublicProperty { get; set; }
                    private int PrivateProperty { get; set; }
                    protected int ProtectedProperty { get; set; }
                    internal int InternalProperty { get; set; }
                }
                """,
                """
                interface PublicOnlyTest {
                    publicProperty: number;
                }
                """, DateType.Union, true, true, false);
        }

        [TestMethod]
        public void ConvertTestNotPubicOnly()
        {
            this.ConvertTestCore("""
                public class NotPublicOnlyTest
                {
                    public int PublicProperty { get; set; }
                    private int PrivateProperty { get; set; }
                    protected int ProtectedProperty { get; set; }
                    internal int InternalProperty { get; set; }
                }
                class PrivateTest
                {
                    public int PublicProperty { get; set; }
                    private int PrivateProperty { get; set; }
                    protected int ProtectedProperty { get; set; }
                    internal int InternalProperty { get; set; }
                }
                """,
                """
                interface NotPublicOnlyTest {
                    publicProperty: number;
                    privateProperty: number;
                    protectedProperty: number;
                    internalProperty: number;
                }

                interface PrivateTest {
                    publicProperty: number;
                    privateProperty: number;
                    protectedProperty: number;
                    internalProperty: number;
                }
                """, DateType.Union, true, false, false);
        }

        [TestMethod]
        public void ConvertTestAddExport()
        {
            this.ConvertTestCore("""
                public class ExportTest
                {
                    public int Id { get; set; }
                    public string Name { get; set; }
                }
                """,
                """
                export interface ExportTest {
                    id: number;
                    name: string;
                }
                """, DateType.Union, true, true, true);
        }

        [TestMethod]
        public void ConvertTestComplex()
        {
            this.ConvertTestCore("""
                public interface Item
                {
                    public int Id { get; set; }
                    public string Name { get; set; }
                }

                public class ComplexTest
                {
                    public int Id { get; set; }
                    public string Name { get; set; }
                    public DateTime CreatedAt { get; set; }
                    public List<string> Tags { get; set; }
                    public Dictionary<string, int> Metadata { get; set; }
                    public (int Id, string Name) SimpleTuple { get; set; }
                    public Item Item { get; set; }
                    public List<Item> Items { get; set; }
                    public Dictionary<string, Item> ItemDictionary { get; set; }
                }
                """,
                """
                interface Item {
                    id: number;
                    name: string;
                }

                interface ComplexTest {
                    id: number;
                    name: string;
                    createdAt: Date | string;
                    tags: string[];
                    metadata: {[key: string]: number};
                    simpleTuple: [number, string];
                    item: Item;
                    items: Item[];
                    itemDictionary: {[key: string]: Item};
                }
                """, DateType.Union, true, true, false);
        }
    }
}