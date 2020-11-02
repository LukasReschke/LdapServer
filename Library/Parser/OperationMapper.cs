using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using LdapServer.Models.Operations;
using LdapServer.Parser.Decoder;
using LdapServer.Parser.Encoder;

namespace LdapServer.Parser
{
    internal class OperationMapper
    {
        private Dictionary<int, Type> OperationTypeMapper = new Dictionary<int, Type>();
        private Dictionary<int, Type> DecoderTypeMapper = new Dictionary<int, Type>();
        private Dictionary<int, Type> EncoderTypeMapper = new Dictionary<int, Type>();

        internal OperationMapper()
        {
            PopulateOperationTypeMapper();
            PopulateDecoderTypeMapper();
            PopulateEncoderTypeMapper();
        }

        private void PopulateDecoderTypeMapper()
        {
            IEnumerable<Type> types = from t in Assembly.GetExecutingAssembly().GetTypes()
                                      where t.GetInterfaces().Any(i =>
                                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IApplicationDecoder<>)
                                      )
                                      select t;

            types.ToList().ForEach(t =>
            {
                Type operationType = t.GetInterfaces()[0].GenericTypeArguments[0];
                KeyValuePair<int, Type> mappedOperation = OperationTypeMapper.First(x => x.Value == operationType);

                DecoderTypeMapper.Add(mappedOperation.Key, t);
            });
        }

        private void PopulateEncoderTypeMapper()
        {
            IEnumerable<Type> types = from t in Assembly.GetExecutingAssembly().GetTypes()
                                      where t.GetInterfaces().Any(i =>
                                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IApplicationEncoder<>)
                                      )
                                      select t;

            types.ToList().ForEach(t =>
            {
                Type operationType = t.GetInterfaces()[0].GenericTypeArguments[0];
                KeyValuePair<int, Type> mappedOperation = OperationTypeMapper.First(x => x.Value == operationType);

                EncoderTypeMapper.Add(mappedOperation.Key, t);
            });
        }

        private void PopulateOperationTypeMapper()
        {
            IEnumerable<Type> types = from t in Assembly.GetExecutingAssembly().GetTypes()
                                      where t.IsClass
                                      where typeof(IProtocolOp).IsAssignableFrom(t)
                                      select t;

            types.ToList().ForEach(t =>
            {
                IProtocolOp protocolOp = (IProtocolOp)FormatterServices.GetUninitializedObject(t);
                int tag = protocolOp.GetTag();
                OperationTypeMapper.Add(tag, t);
            });
        }

        internal Type GetDecoderForTag(int tag)
        {
            return DecoderTypeMapper.Single(t => t.Key == tag).Value;
        }

        internal Type GetEncoderForTag(int tag)
        {
            return EncoderTypeMapper.Single(t => t.Key == tag).Value;
        }
    }
}