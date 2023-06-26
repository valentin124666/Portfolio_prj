using System;

namespace Core
{
    public class OverrideTypes : Attribute
    {
        public Type Presenter { get; }
        public Type View { get; }

        public OverrideTypes(Type presenter, Type view)
        {
            Presenter = presenter;
            View = view;
        }
    }
}