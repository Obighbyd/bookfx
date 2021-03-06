namespace BookFx.Tests.Cores
{
    using BookFx.Cores;
    using FluentAssertions;
    using Xunit;
    using static BookFx.Functional.F;

    public class BoxCoreExtTests
    {
        private const string ProtoRangeName = "Header";

        [Fact]
        public void SelfAndDescendants_Leaf_Self()
        {
            var box = Make.Value().Get;

            var result = box.SelfAndDescendants();

            result.Should().Equal(box);
        }

        [Fact]
        public void SelfAndDescendants_EmptyComposite_Self()
        {
            var box = Make.Row().Get;

            var result = box.SelfAndDescendants();

            result.Should().Equal(box);
        }

        [Fact]
        public void SelfAndDescendants_TwoChildren_SelfAndChildren()
        {
            var child1 = BoxCore.Create(BoxType.Value);
            var child2 = BoxCore.Create(BoxType.Value);
            var box = BoxCore.Create(BoxType.Row).Add(List(child1, child2));

            var result = box.SelfAndDescendants();

            result.Should().Equal(box, child1, child2);
        }

        [Fact]
        public void SelfAndDescendants_3Gens_Xyz()
        {
            var genZ = BoxCore.Create(BoxType.Value);
            var genY = BoxCore.Create(BoxType.Row).Add(List(genZ));
            var genX = BoxCore.Create(BoxType.Row).Add(List(genY));

            var result = genX.SelfAndDescendants();

            result.Should().Equal(genX, genY, genZ);
        }

        [Fact]
        public void SelfAndDescendants_EmptyProtoBox_Self()
        {
            var box = Make.Proto(new byte[0], ProtoRangeName).Get;

            var result = box.SelfAndDescendants();

            result.Should().Equal(box);
        }

        [Fact]
        public void SelfAndDescendants_ProtoBoxWithTwoSlots_SelfAndSlots()
        {
            var box = GetProtoBoxWithTwoSlots();

            var result = box.SelfAndDescendants();

            result.Should().Equal(box, box.Slots[0].Box, box.Slots[1].Box);
        }

        [Fact]
        public void Descendants_Leaf_Empty()
        {
            var box = Make.Value().Get;

            var result = box.Descendants();

            result.Should().BeEmpty();
        }

        [Fact]
        public void Descendants_EmptyComposite_Empty()
        {
            var box = Make.Row().Get;

            var result = box.Descendants();

            result.Should().BeEmpty();
        }

        [Fact]
        public void Descendants_TwoChildren_Children()
        {
            var child1 = BoxCore.Create(BoxType.Value);
            var child2 = BoxCore.Create(BoxType.Value);
            var box = BoxCore.Create(BoxType.Row).Add(List(child1, child2));

            var result = box.Descendants();

            result.Should().Equal(child1, child2);
        }

        [Fact]
        public void Descendants_3Gens_Уz()
        {
            var genZ = BoxCore.Create(BoxType.Value);
            var genY = BoxCore.Create(BoxType.Row).Add(List(genZ));
            var genX = BoxCore.Create(BoxType.Row).Add(List(genY));

            var result = genX.Descendants();

            result.Should().Equal(genY, genZ);
        }

        [Fact]
        public void Descendants_EmptyProtoBox_Empty()
        {
            var box = Make.Proto(new byte[0], ProtoRangeName).Get;

            var result = box.Descendants();

            result.Should().BeEmpty();
        }

        [Fact]
        public void Descendants_ProtoBoxWithTwoSlots_SlotBoxes()
        {
            var box = GetProtoBoxWithTwoSlots();

            var result = box.Descendants();

            result.Should().Equal(box.Slots[0].Box, box.Slots[1].Box);
        }

        [Fact]
        public void ImmediateDescendants_Leaf_Empty()
        {
            var box = Make.Value().Get;

            var result = box.ImmediateDescendants();

            result.Should().BeEmpty();
        }

        [Fact]
        public void ImmediateDescendants_EmptyComposite_Empty()
        {
            var box = Make.Row().Get;

            var result = box.ImmediateDescendants();

            result.Should().BeEmpty();
        }

        [Fact]
        public void ImmediateDescendants_TwoChildren_Children()
        {
            var child1 = BoxCore.Create(BoxType.Value);
            var child2 = BoxCore.Create(BoxType.Value);
            var box = BoxCore.Create(BoxType.Row).Add(List(child1, child2));

            var result = box.ImmediateDescendants();

            result.Should().Equal(child1, child2);
        }

        [Fact]
        public void ImmediateDescendants_3Gens_У()
        {
            var genZ = BoxCore.Create(BoxType.Value);
            var genY = BoxCore.Create(BoxType.Row).Add(List(genZ));
            var genX = BoxCore.Create(BoxType.Row).Add(List(genY));

            var result = genX.ImmediateDescendants();

            result.Should().Equal(genY);
        }

        [Fact]
        public void ImmediateDescendants_EmptyProtoBox_Empty()
        {
            var box = Make.Proto(new byte[0], ProtoRangeName).Get;

            var result = box.ImmediateDescendants();

            result.Should().BeEmpty();
        }

        [Fact]
        public void ImmediateDescendants_ProtoBoxWithTwoSlots_SlotBoxes()
        {
            var box = GetProtoBoxWithTwoSlots();

            var result = box.ImmediateDescendants();

            result.Should().Equal(box.Slots[0].Box, box.Slots[1].Box);
        }

        private static BoxCore GetProtoBoxWithTwoSlots() =>
            Make
                .Proto(new byte[0], ProtoRangeName)
                .Add("Child1", BoxCore.Create(BoxType.Value))
                .Add("Child2", BoxCore.Create(BoxType.Value))
                .Get;
    }
}