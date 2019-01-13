﻿using Castle.DynamicProxy;

namespace Dawnx
{
    public static class DawnIInterceptor
    {
        public static T Proxy<T>(this T @this)
            where T : class, IInterceptor, new()
        {
            return new ProxyGenerator().CreateClassProxy<T>(new T());
        }

        public static T Proxy<T, TIntercetor>(this T @this, TIntercetor Intercetor)
            where T : class
            where TIntercetor : class, IInterceptor
        {
            return new ProxyGenerator().CreateClassProxy<T>(Intercetor);
        }

    }
}
