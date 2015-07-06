using System;
using System.IO;
using System.Threading.Tasks;

using Google.Apis.Util.Store;
using Google.Apis.Json;
namespace googlecloud1.login
{
    public class FileDataStore : IDataStore
    {
        readonly string folderPath;
        /// <summary>폴더의 경로.</summary>
        public string FolderPath { get { return folderPath; } }

        /// <summary>
        /// file data store의 생성자 (저장될 폴더의 경로 설정)
        /// </summary>
        /// <param name="folder">저장될 폴더의 경로</param>
        /// <param name="fullPath">
        /// 절대적인 혹은 상대적인 전체 경로 (기본값 false)
        /// <see cref="Environment.SpecialFolder.ApplicationData"/>.
        /// </param>
        public FileDataStore(string folder, bool fullPath = false)
        {
            folderPath = fullPath
                ? folder
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// 얻어온 키와 값으로 새로운 파일을 만들어서 저장한다<see cref="GenerateStoredKey"/>) in 
        /// <see cref="FolderPath"/>.
        /// </summary>
        /// <typeparam name="T">저장될 데이터의 타입형식.</typeparam>
        /// <param name="key">key값 저장 데이터의 분류를 위한 key</param>
        /// <param name="value">저장될 데이터의 내부 값 (ex:토큰이나 코드 같은 종류의 값들).</param>
        public Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var serialized = NewtonsoftJsonSerializer.Instance.Serialize(value);
            var filePath = Path.Combine(folderPath, GenerateStoredKey(key, typeof(T)));
            File.WriteAllText(filePath, serialized);
            return TaskEx.Delay(0);
        }

        /// <summary>
        ///  키값을 이용하여 정해진 파일안에 데이터를 삭제한다.
        /// <see cref="FolderPath"/>.
        /// </summary>
        /// <param name="key">삭제할 데이터의 key값</param>
        public Task DeleteAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var filePath = Path.Combine(folderPath, GenerateStoredKey(key, typeof(T)));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return TaskEx.Delay(0);
        }

        /// <summary>
        /// 매개 변수로 넘어온 키값을 이용하여 파일안의 데이터를 가져온다
        /// </summary>
        /// <typeparam name="T">가져올 데이터의 타입형식</typeparam>
        /// <param name="key">가져올 데이터의 key값</param>
        /// <returns>저장된 데이터를 반환</returns>
        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            var filePath = Path.Combine(folderPath, GenerateStoredKey(key, typeof(T)));
            if (File.Exists(filePath))
            {
                try
                {
                    var obj = File.ReadAllText(filePath);
                    tcs.SetResult(NewtonsoftJsonSerializer.Instance.Deserialize<T>(obj));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }
            else
            {
                tcs.SetResult(default(T));
            }
            return tcs.Task;
        }

        /// <summary>
        /// 모든 파일안의 데이터를 전부 지운다
        /// </summary>
        public Task ClearAsync()
        {
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
                Directory.CreateDirectory(folderPath);
            }

            return TaskEx.Delay(0);
        }

        /// <summary>클래스 타입과 넘어온 key값을 이용하여 중복되지 않은 유니크한 key를 만든다</summary>
        /// <param name="key">데이터의 key값.</param>
        /// <param name="t">저장 타입</param>
        public static string GenerateStoredKey(string key, Type t)
        {
            return string.Format("{0}-{1}", t.FullName, key);
        }
    }
}
