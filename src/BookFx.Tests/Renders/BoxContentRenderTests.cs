namespace BookFx.Tests.Renders
{
    using System;
    using BookFx.Epplus;
    using BookFx.Renders;
    using BookFx.Tests.Arbitraries;
    using FluentAssertions;
    using FsCheck;
    using FsCheck.Xunit;
    using OfficeOpenXml;
    using Xunit;

    [Properties(MaxTest = 10)]
    public class BoxContentRenderTests
    {
        private const string OldValue = "old value";
        private const string OldFormula = "\"old formula\"";

        [Fact]
        public void ContentRender_NoValue_ValueNotSet() =>
            CheckValue(
                Make.Value(),
                range => range.Value.Should().Be(OldValue));

        [Fact]
        public void ContentRender_NoValue_FormulaNotSet() =>
            CheckFormula(
                Make.Value(),
                range => range.Formula.Should().Be(OldFormula));

        [Property]
        public void ContentRender_EscapedValue_Set(NonNull<string> content) =>
            CheckValue(
                Make.Value($"'{content.Get}"),
                range => range.Value.Should().Be(content.Get));

        [Property(Arbitrary = new[] { typeof(GeneralStringValueArb) })]
        public void ContentRender_Object_Set(string content) =>
            CheckValue(
                Make.Value(content),
                range => range.Value.Should().Be(content));

        [Property]
        public void ContentRender_Int_Set(int content) =>
            CheckValue(
                Make.Value(content),
                range => range.Value.Should().Be(content));

        [Theory]
        [InlineData("=1+2", "1+2")]
        [InlineData("=1 + 2", "1 + 2")]
        [InlineData("=RC", "E4")]
        [InlineData("=R2C3", "$C$2")]
        [InlineData("=R[2]C[3]", "H6")]
        [InlineData("=SUM(R[1]C,R[10]C)", "SUM(E5,E14)")]
        [InlineData("=SUM(R[1]C:R[10]C)", "SUM(E5:E14)")]
        [InlineData("=SUM(Data C)", "SUM(Data E:E)")]
        [InlineData("=SUM(Data1 Data2 Data3)", "SUM(Data1 Data2 Data3)")]
        [InlineData("=SUM(R Data1 Data2 Data3)", "SUM(4:4 Data1 Data2 Data3)")]
        [InlineData("=SUM(R1 Data1 Data2 Data3)", "SUM($1:$1 Data1 Data2 Data3)")]
        public void ContentRender_ValidFormulas_SetExpected(string formula, string expected) =>
            CheckFormula(
                Make.Value(formula),
                range => range.Formula.Should().Be(expected));

        [Fact]
        public void ContentRender_SpanAndNoMerge_FirstCellOnly() =>
            Packer.OnSheet(excelSheet =>
            {
                var excelRange = excelSheet.Cells[1, 1, 2, 2];

                Make.Value("content").Merge(false).Get.ContentRender()(excelRange);

                excelSheet.Cells[1, 1].Value.Should().NotBeNull();
                excelSheet.Cells[1, 2].Value.Should().BeNull();
                excelSheet.Cells[2, 1].Value.Should().BeNull();
                excelSheet.Cells[2, 2].Value.Should().BeNull();
            });

        private static void CheckValue(ValueBox box, Action<ExcelRange> assertion) =>
            Packer.OnSheet(excelSheet =>
            {
                var excelRange = excelSheet.Cells[1, 1];
                excelRange.Value = OldValue;

                box.Get.ContentRender()(excelRange);

                assertion(excelRange);
            });

        private static void CheckFormula(ValueBox box, Action<ExcelRange> assertion) =>
            Packer.OnSheet(excelSheet =>
            {
                var excelRange = excelSheet.Cells[Row: 4, Col: 5];
                excelRange.Formula = OldFormula;

                box.Get.ContentRender()(excelRange);

                assertion(excelRange);
            });
    }
}