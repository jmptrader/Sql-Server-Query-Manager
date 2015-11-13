﻿/*
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.Data;

namespace CrowCreek.Utilities
{
  public static class IDataRecordExtensions
  {
    public static TFieldType ReadField<TFieldType>(this IDataRecord dataRecord, string fieldName)
    {
      var fieldType = typeof(TFieldType);

      object raw;
      try
      {
        raw = dataRecord[fieldName];
      }
      catch (Exception ex)
      {
        throw new FieldReadException(fieldName, ex);
      }
      // If the value is an enumeration check that the value from the db is defined in the enum
      if (fieldType.IsEnum && !Enum.IsDefined(fieldType, raw)) throw new FieldEnumNotDefinedException(fieldName, fieldType, raw.ToString());
      // If we got a null from the database, check to see that we are reading a reference type or a nullable value type, if so return a default of the type
      if (raw == DBNull.Value && (!fieldType.IsValueType || Nullable.GetUnderlyingType(fieldType) != null))
      {
        return default(TFieldType);
      }
      try
      {
        return (TFieldType)raw;
      }
      catch (InvalidCastException ex)
      {
        throw new FieldReadCastException(fieldName, ex);
      }
      catch (Exception ex)
      {
        throw new FieldReadException(fieldName, ex);
      }
    }
  }
}
