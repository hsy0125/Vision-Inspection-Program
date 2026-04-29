namespace InspectionProgram.Common
{
    /// <summary>
    /// 세션 내 티칭에서 저장한 <see cref="InspectionRecipe"/> 보관소.
    /// </summary>
    public static class TeachingInspectionRecipeStore
    {
        private static InspectionRecipe _current;

        public static void Set(InspectionRecipe recipe)
        {
            _current = recipe;
        }

        public static bool TryGet(out InspectionRecipe recipe)
        {
            recipe = _current;
            return recipe != null && !recipe.IsEmpty;
        }

        public static void Clear()
        {
            _current = null;
        }
    }
}
