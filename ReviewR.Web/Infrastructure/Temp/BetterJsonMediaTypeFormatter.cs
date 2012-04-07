using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.Internal;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Temp;

namespace System.Net.Http.Formatting
{
    /// <summary>
    /// <see cref="MediaTypeFormatter"/> class to handle Json.
    /// </summary>
    public class BetterJsonMediaTypeFormatter : MediaTypeFormatter
    {
        private static readonly MediaTypeHeaderValue[] _supportedMediaTypes = new MediaTypeHeaderValue[]
        {
            new MediaTypeHeaderValue("application/json"),
            new MediaTypeHeaderValue("text/json")
        };

        private JsonSerializerSettings _jsonSerializerSettings;
        private readonly IContractResolver _defaultContractResolver;
        private int _maxDepth = FormattingUtilities.DefaultMaxDepth;
        
        private RequestHeaderMapping _requestHeaderMapping;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMediaTypeFormatter"/> class.
        /// </summary>
        public BetterJsonMediaTypeFormatter()
        {
            // Set default supported media types
            foreach (MediaTypeHeaderValue value in _supportedMediaTypes)
            {
                SupportedMediaTypes.Add(value);
            }

            // Initialize serializer
            _defaultContractResolver = new JsonContractResolver(this);
            _jsonSerializerSettings = CreateDefaultSerializerSettings();

            // Set default supported character encodings

            _requestHeaderMapping = new XHRRequestHeaderMapping();
            MediaTypeMappings.Add(_requestHeaderMapping);
        }

        /// <summary>
        /// Gets the default media type for Json, namely "application/json".
        /// </summary>
        /// <remarks>
        /// The default media type does not have any <c>charset</c> parameter as 
        /// the <see cref="Encoding"/> can be configured on a per <see cref="JsonMediaTypeFormatter"/> 
        /// instance basis.
        /// </remarks>
        /// <value>
        /// Because <see cref="MediaTypeHeaderValue"/> is mutable, the value
        /// returned will be a new instance every time.
        /// </value>
        public static MediaTypeHeaderValue DefaultMediaType
        {
            get { return new MediaTypeHeaderValue("application/json"); }
        }

        /// <summary>
        /// Gets or sets the <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.
        /// </summary>
        public JsonSerializerSettings SerializerSettings
        {
            get { return _jsonSerializerSettings; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _jsonSerializerSettings = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to indent elements when writing data. 
        /// </summary>
        public bool Indent { get; set; }

        /// <summary>
        /// Gets or sets the maximum depth allowed by this formatter.
        /// </summary>
        public int MaxDepth
        {
            get
            {
                return _maxDepth;
            }
            set
            {
                _maxDepth = value;
            }
        }

        /// <summary>
        /// Creates a <see cref="JsonSerializerSettings"/> instance with the default settings used by the <see cref="JsonMediaTypeFormatter"/>.
        /// </summary>
        public JsonSerializerSettings CreateDefaultSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                ContractResolver = _defaultContractResolver,
                MissingMemberHandling = MissingMemberHandling.Ignore,

                // Do not change this setting
                // Setting this to None prevents Json.NET from loading malicious, unsafe, or security-sensitive types
                TypeNameHandling = TypeNameHandling.None
            };
        }

        /// <summary>
        /// Determines whether this <see cref="JsonMediaTypeFormatter"/> can read objects
        /// of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object that will be read.</param>
        /// <returns><c>true</c> if objects of this <paramref name="type"/> can be read, otherwise <c>false</c>.</returns>
        protected override bool CanReadType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return true;
        }

        /// <summary>
        /// Determines whether this <see cref="JsonMediaTypeFormatter"/> can write objects
        /// of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object that will be written.</param>
        /// <returns><c>true</c> if objects of this <paramref name="type"/> can be written, otherwise <c>false</c>.</returns>
        protected override bool CanWriteType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return true;
        }

        /// <summary>
        /// Called during deserialization to read an object of the specified <paramref name="type"/>
        /// from the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="type">The type of object to read.</param>
        /// <param name="stream">The <see cref="Stream"/> from which to read.</param>
        /// <param name="contentHeaders">The <see cref="HttpContentHeaders"/> for the content being written.</param>
        /// <param name="formatterLogger">The <see cref="IFormatterLogger"/> to log events to.</param>
        /// <returns>A <see cref="Task"/> whose result will be the object instance that has been read.</returns>
        protected override Task<object> OnReadFromStreamAsync(Type type, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var t = new TaskCompletionSource<object>();
            t.SetResult(new Func<object>(() =>
            {
                // If content length is 0 then return default value for this type
                if (contentHeaders != null && contentHeaders.ContentLength == 0)
                {
                    return null;
                }

                // Get the character encoding for the content
                Encoding effectiveEncoding = Encoding.UTF8;

                try
                {
                    using (JsonTextReader jsonTextReader = new JsonTextReader(new StreamReader(stream, effectiveEncoding)) { CloseInput = false })
                    {
                        JsonSerializer jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);
                        return jsonSerializer.Deserialize(jsonTextReader, type);
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            })());
            return t.Task;
        }

        /// <summary>
        /// Called during serialization to write an object of the specified <paramref name="type"/>
        /// to the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="type">The type of object to write.</param>
        /// <param name="value">The object to write.</param>
        /// <param name="stream">The <see cref="Stream"/> to which to write.</param>
        /// <param name="contentHeaders">The <see cref="HttpContentHeaders"/> for the content being written.</param>
        /// <param name="transportContext">The <see cref="TransportContext"/>.</param>
        /// <returns>A <see cref="Task"/> that will write the value to the stream.</returns>
        protected override Task OnWriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext, TransportContext transportContext)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            Encoding effectiveEncoding = Encoding.UTF8;

            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(new StreamWriter(stream, effectiveEncoding)) { CloseOutput = false })
            {
                if (Indent)
                {
                    jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                }
                JsonSerializer jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);
                jsonSerializer.Serialize(jsonTextWriter, value);
                jsonTextWriter.Flush();
            }

            var s = new TaskCompletionSource<AsyncVoid>();
            s.SetResult(new AsyncVoid());
            return s.Task;
        }

        internal struct AsyncVoid { }

        private static bool IsKnownUnserializableType(Type type)
        {
            if (type.IsGenericType)
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return IsKnownUnserializableType(type.GetGenericArguments()[0]);
                }
            }

            if (!type.IsVisible)
            {
                return true;
            }

            if (type.HasElementType && IsKnownUnserializableType(type.GetElementType()))
            {
                return true;
            }

            return false;
        }
    }
}
