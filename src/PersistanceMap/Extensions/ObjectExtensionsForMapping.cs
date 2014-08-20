using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace PersistanceMap
{
    /// <summary>
    /// Class containing extensions methods for object that are solely needed for mapping from IDataReader to objects
    /// </summary>
    internal static class ObjectExtensionsForMapping
    {
        public static T PopulateFromReader<T>(this T objWithProperties, IReaderContext context, FieldDefinition[] fieldDefs, Dictionary<string, int> indexCache)
        {
            try
            {
                foreach (var fieldDef in fieldDefs)
                {
                    int index;
                    if (indexCache != null)
                    {
                        if (!indexCache.TryGetValue(fieldDef.MemberName, out index))
                        {
                            index = context.DataReader.GetColumnIndex(fieldDef.FieldName);
                            //if (index == NotFound)
                            //{
                            //    index = TryGuessColumnIndex(fieldDef.FieldName, dataReader);
                            //}

                            indexCache.Add(fieldDef.MemberName, index);
                        }
                    }
                    else
                    {
                        index = context.DataReader.GetColumnIndex(fieldDef.FieldName);
                        //if (index == NotFound)
                        //{
                        //    index = TryGuessColumnIndex(fieldDef.FieldName, dataReader);
                        //}
                    }

                    context.SetValue(fieldDef, index, objWithProperties);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return objWithProperties;
        }

        public static void PopulateFromReader(this Dictionary<string, object> row, IReaderContext context, IEnumerable<ObjectDefinition> objectDefs, Dictionary<string, int> indexCache)
        {
            try
            {
                foreach (var def in objectDefs)
                {
                    int index;
                    if (indexCache != null)
                    {
                        if (!indexCache.TryGetValue(def.Name, out index))
                        {
                            index = context.DataReader.GetColumnIndex(def.Name);

                            indexCache.Add(def.Name, index);
                        }
                    }
                    else
                    {
                        index = context.DataReader.GetColumnIndex(def.Name);
                    }

                    row[def.Name] = context.GetValue(def, index);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        //public static void AddSetters<T>(this object anonymous)
        //{
        //    //http://stackoverflow.com/questions/123540/modifying-existing-net-assemblies
        //    //One important point: if the assembly is signed, any changes will fail and you'll end up with a dud.

        //    var properties = typeof(T).GetProperties();
        //    foreach (var prop in properties)
        //    {
        //        //prop.PropertyType
        //        PropertyBuilder pbNewProp = tb.DefineProperty(prop.Name, PropertyAttributes.HasDefault, prop.PropertyType, null);
        //        MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        //        //// Define the "get" accessor method
        //        //MethodBuilder mbNewPropGetAccessor = tb.DefineMethod(
        //        //    "get_NewProp",
        //        //    getSetAttr,
        //        //    typeof(int),
        //        //    Type.EmptyTypes);

        //        //ILGenerator NewPropGetIL = mbNewPropGetAccessor.GetILGenerator();
        //        //NewPropGetIL.Emit(OpCodes.Ldarg_0);
        //        //NewPropGetIL.Emit(OpCodes.Ldc_I4_1);
        //        //NewPropGetIL.Emit(OpCodes.Call, typeof(Test).GetMethod("GetVal"));
        //        //NewPropGetIL.Emit(OpCodes.Ret);

        //        // Define the "set" accessor method 
        //        MethodBuilder mbNewPropSetAccessor = tb.DefineMethod(
        //            "set_NewProp",
        //            getSetAttr,
        //            null,
        //            new Type[] { typeof(int) });

        //        ILGenerator NewPropSetIL = mbNewPropSetAccessor.GetILGenerator();
        //        NewPropSetIL.Emit(OpCodes.Ldarg_0);
        //        NewPropSetIL.Emit(OpCodes.Ldc_I4_1);
        //        NewPropSetIL.Emit(OpCodes.Ldarg_1);
        //        NewPropSetIL.Emit(OpCodes.Call, typeof(T).GetMethod("SetVal"));
        //        NewPropSetIL.Emit(OpCodes.Ret);

        //        // Map the accessor methods
        //        //pbNewProp.SetGetMethod(mbNewPropGetAccessor);
        //        pbNewProp.SetSetMethod(mbNewPropSetAccessor);
        //    }
        //}

        /*
        private static int TryGuessColumnIndex(string fieldName, IDataReader dataReader)
        {
            var fieldCount = dataReader.FieldCount;
            for (var i = 0; i < fieldCount; i++)
            {
                var dbFieldName = dataReader.GetName(i);

                // First guess: Maybe the DB field has underscores? (most common)
                // e.g. CustomerId (C#) vs customer_id (DB)
                var dbFieldNameWithNoUnderscores = dbFieldName.Replace("_", "");
                if (String.Compare(fieldName, dbFieldNameWithNoUnderscores, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return i;
                }

                // Next guess: Maybe the DB field has special characters?
                // e.g. Quantity (C#) vs quantity% (DB)
                var dbFieldNameSanitized = AllowedPropertyCharsRegex.Replace(dbFieldName, String.Empty);
                if (String.Compare(fieldName, dbFieldNameSanitized, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return i;
                }

                // Next guess: Maybe the DB field has special characters *and* has underscores?
                // e.g. Quantity (C#) vs quantity_% (DB)
                if (String.Compare(fieldName, dbFieldNameSanitized.Replace("_", String.Empty), StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return i;
                }

                // Next guess: Maybe the DB field has some prefix that we don't have in our C# field?
                // e.g. CustomerId (C#) vs t130CustomerId (DB)
                if (dbFieldName.EndsWith(fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }

                // Next guess: Maybe the DB field has some prefix that we don't have in our C# field *and* has underscores?
                // e.g. CustomerId (C#) vs t130_CustomerId (DB)
                if (dbFieldNameWithNoUnderscores.EndsWith(fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }

                // Next guess: Maybe the DB field has some prefix that we don't have in our C# field *and* has special characters?
                // e.g. CustomerId (C#) vs t130#CustomerId (DB)
                if (dbFieldNameSanitized.EndsWith(fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }

                // Next guess: Maybe the DB field has some prefix that we don't have in our C# field *and* has underscores *and* has special characters?
                // e.g. CustomerId (C#) vs t130#Customer_I#d (DB)
                if (dbFieldNameSanitized.Replace("_", "").EndsWith(fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }

                // Cater for Naming Strategies like PostgreSQL that has lower_underscore names
                if (dbFieldNameSanitized.Replace("_", "").EndsWith(fieldName.Replace("_", ""), StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return NotFound;
        }
        */
    }
}
