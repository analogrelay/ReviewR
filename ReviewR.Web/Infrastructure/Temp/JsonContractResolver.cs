﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace System.Net.Http.Formatting
{
    // Default Contract resolver for JsonMediaTypeFormatter
    // Handles types that DCJS supports, but Json.NET doesn't support out of the box (like [Serializable])
    // Uses the IRequiredMemberSelector to choose required members
    internal class JsonContractResolver : DefaultContractResolver
    {
        private const BindingFlags AllInstanceMemberFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private readonly MediaTypeFormatter _formatter;

        public JsonContractResolver(MediaTypeFormatter formatter)
        {
            _formatter = formatter;
        }

        protected override JsonObjectContract CreateObjectContract(Type type)
        {
            JsonObjectContract contract = base.CreateObjectContract(type);

            // Handling [Serializable] types
            if (type.IsSerializable && !IsTypeNullable(type) && !IsTypeJsonObject(type))
            {
                contract.Properties.Clear();
                foreach (JsonProperty property in CreateSerializableJsonProperties(type))
                {
                    contract.Properties.Add(property);
                }
            }

            return contract;
        }

        // Determines whether a member is required or not and sets the appropriate JsonProperty settings
        private void ConfigureProperty(MemberInfo member, JsonProperty property)
        {
            property.Required = Required.Default;
            property.DefaultValueHandling = DefaultValueHandling.Ignore;
            property.NullValueHandling = NullValueHandling.Ignore;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            ConfigureProperty(member, property);
            return property;
        }

        private IEnumerable<JsonProperty> CreateSerializableJsonProperties(Type type)
        {
            return type.GetFields(AllInstanceMemberFlag)
                .Where(field => !field.IsNotSerialized)
                .Select(field =>
                {
                    JsonProperty property = PrivateMemberContractResolver.Instance.CreatePrivateProperty(field, MemberSerialization.OptOut);
                    ConfigureProperty(field, property);
                    return property;
                });
        }

        private static bool IsTypeNullable(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        private static bool IsTypeJsonObject(Type type)
        {
            return type.GetCustomAttributes(typeof(JsonObjectAttribute), false).Any();
        }

        private class PrivateMemberContractResolver : DefaultContractResolver
        {
            internal static PrivateMemberContractResolver Instance = new PrivateMemberContractResolver();

            internal PrivateMemberContractResolver()
            {
                DefaultMembersSearchFlags = JsonContractResolver.AllInstanceMemberFlag;
            }

            internal JsonProperty CreatePrivateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                return CreateProperty(member, memberSerialization);
            }
        }
    }
}
