using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MemoryLeak.Controllers
{
    /// <summary>
    /// Main testing controller
    /// </summary>
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private static ConcurrentBag<string> _staticStrings = new ConcurrentBag<string>();
        private static readonly HttpClient _httpClient = new HttpClient();
        private static ArrayPool<byte> _arrayPool = ArrayPool<byte>.Create();

        public ApiController()
        {
            Interlocked.Increment(ref DiagnosticsController.Requests);
        }

        /// <summary>
        /// Generates a big string and saves it in the static collection
        /// </summary>
        /// <returns></returns>
        [HttpGet("staticstring")]
        public ActionResult<string> GetStaticString()
        {
            var bigString = new string('x', 10 * 1024);
            _staticStrings.Add(bigString);
            return bigString;
        }

        /// <summary>
        /// Release the static collection usage
        /// </summary>
        /// <returns></returns>
        [HttpPost("ReleaseStaticString")]
        public ActionResult ReleaseStaticString()
        {
            _staticStrings.Clear();
            return Ok();
        }

        /// <summary>
        /// Generates a big string 
        /// </summary>
        /// <returns></returns>
        [HttpGet("bigstring")]
        public ActionResult<string> GetBigString()
        {
            return new string('x', 10 * 1024);
        }

        /// <summary>
        /// Generates a byte array of the set size  
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("loh/{size=85000}")]
        public int GetLOH(int size)
        {
            return new byte[size].Length;
        }

        /// <summary>
        /// Allocates new HttpClient and send the request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet("httpclient1")]
        public async Task<int> GetHttpClient1(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var result = await httpClient.GetAsync(url);
                return (int)result.StatusCode;
            }
        }

        /// <summary>
        /// Sends the request by prepared static client
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet("httpclient2")]
        public async Task<int> GetHttpClient2(string url)
        {
            var result = await _httpClient.GetAsync(url);
            return (int)result.StatusCode;
        }

        /// <summary>
        /// Generates an array of the set size
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("array/{size}")]
        public byte[] GetArray(int size)
        {
            var array = new byte[size];

            var random = new Random();
            random.NextBytes(array);

            return array;
        }

        /// <summary>
        /// Generates an array using shared memory - a leak is the result
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("pooledarray/{size}")]
        public byte[] GetPooledArray(int size)
        {
            var pooledArray = new PooledArray(size);

            var random = new Random();
            random.NextBytes(pooledArray.Array);

            HttpContext.Response.RegisterForDispose(pooledArray);

            return pooledArray.Array;
        }

        private class PooledArray : IDisposable
        {
            public byte[] Array { get; private set; }

            public PooledArray(int size)
            {
                Array = _arrayPool.Rent(size);
            }

            public void Dispose()
            {
                _arrayPool.Return(Array);
            }
        }
    }
}
