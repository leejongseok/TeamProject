﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Flows;
namespace googlecloud1.login
{
    class GoogleWebAuthorization : GoogleWebAuthorizationBroker
    {
        /// <summary>
        /// 로그인 처리를 위한 초기설정을 도와주는 함수
        /// </summary>
        /// <param name="client">Client_id와 Client_Secret이 담겨있는 클래스(구글 드라이브 콘솔에서 발급받은 id와 비밀번호)</param>
        /// <param name="scopes">접근 권한에 대한 주소값들이 들어있는 List</param>
        /// <param name="username">사용자를 구별하기 위한 닉네임</param>
        /// <param name="taskCancellationToken">작업이 취소되지 않아야 함을 나타내는 값 : None으로 표시</param>
        /// <param name="store">저장할 위치 (경로로 표시 : 기본 위치 -> C:\Users\bit-user\AppData\Roaming) </param>
        /// initializer는 클라이언트 id와 클라이언트 시크릿 번호를 설정해준다.
        /// <returns>반환값은 유저의 정보가 담긴 UserCredential 클래스 반환</returns>
        /// 
        public static async Task<UserCredential> LoginAuthorizationCodeFlowAsync(ClientSecrets client, IEnumerable<string> scopes, string username, CancellationToken taskCancellationToken, FileDataStore store)
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = client,
            };
            return await AuthorizeAsyncCore(initializer, scopes, username, taskCancellationToken, store)
                .ConfigureAwait(false);
        }
        /// <summary>
        /// 저장위치와 접근 권한 설정을 해주는 함수
        /// </summary>
        /// <param name="initializer"> 초기 설정이 담겨있는 클래스</param>
        /// <param name="scopes"> 접근 권한이 담겨있음</param>
        /// <param name="user">사용자를 구별하기 위한 유저 이름 (닉네임)</param>
        /// <param name="taskCancellationToken"> 작업이 취소되지 않아야 함을 나타내는 값 : None으로 표시</param>
        /// <param name="dataStore"> 토큰값이 저장되는 경로 (설정 하지 않으면 null로 값이 들어감  (경로로 표시 : 기본 위치 -> C:\Users\bit-user\AppData\Roaming)) </param>
        /// <returns>반환값은 유저의 정보가 담긴 UserCredential 클래스 반환 </returns>
        private static async Task<UserCredential> AuthorizeAsyncCore(
            GoogleAuthorizationCodeFlow.Initializer initializer, IEnumerable<string> scopes, string user,
            CancellationToken taskCancellationToken, IDataStore dataStore = null)
        {
            initializer.Scopes = scopes;
            initializer.DataStore = dataStore ?? new FileDataStore(Folder);
            GoogleAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(initializer);

            // 토큰을 이용하여 사용자의 정보를 얻어와서 반환해준다.
            return await new googleauth(flow, new LocalServerCodeReceiver()).AuthorizeAsync
                (user, taskCancellationToken).ConfigureAwait(false);
        }
    }
}