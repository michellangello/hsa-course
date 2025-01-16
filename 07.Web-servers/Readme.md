## Cache Testing and Purge Script

This script automates testing the behavior of a caching system (e.g., Nginx with caching enabled) by performing the following steps:
	
    1. Requests one.png and two.png twice to verify caching behavior (cache MISS on the first request, HIT on the second).
	2. Purges the cache for one.png.
	3. Verifies that the cache for one.png is purged (MISS) and confirms that the cache for two.png remains unaffected (HIT).

## Example Output

```
Requesting http://localhost:8083/one.png (expecting MISS)
HTTP/1.1 200 OK
Server: nginx/1.24.0
Date: Thu, 16 Jan 2025 10:39:43 GMT
Content-Type: image/jpeg
Content-Length: 47579
Connection: keep-alive
Last-Modified: Tue, 14 Jan 2025 19:07:07 GMT
ETag: "6786b5db-b9db"
X-Cache-Status: MISS
Accept-Ranges: bytes


Requesting http://localhost:8083/one.png (expecting HIT)
HTTP/1.1 200 OK
Server: nginx/1.24.0
Date: Thu, 16 Jan 2025 10:39:43 GMT
Content-Type: image/jpeg
Content-Length: 47579
Connection: keep-alive
Last-Modified: Tue, 14 Jan 2025 19:07:07 GMT
ETag: "6786b5db-b9db"
X-Cache-Status: HIT
Accept-Ranges: bytes


Requesting http://localhost:8083/two.png (expecting MISS)
HTTP/1.1 200 OK
Server: nginx/1.24.0
Date: Thu, 16 Jan 2025 10:39:43 GMT
Content-Type: image/jpeg
Content-Length: 138460
Connection: keep-alive
Last-Modified: Tue, 14 Jan 2025 19:08:13 GMT
ETag: "6786b61d-21cdc"
X-Cache-Status: MISS
Accept-Ranges: bytes


Requesting http://localhost:8083/two.png (expecting HIT)
HTTP/1.1 200 OK
Server: nginx/1.24.0
Date: Thu, 16 Jan 2025 10:39:43 GMT
Content-Type: image/jpeg
Content-Length: 138460
Connection: keep-alive
Last-Modified: Tue, 14 Jan 2025 19:08:13 GMT
ETag: "6786b61d-21cdc"
X-Cache-Status: HIT
Accept-Ranges: bytes


Purging cache for http://localhost:8083/purge/one.png
HTTP/1.1 200 OK
Server: nginx/1.24.0
Date: Thu, 16 Jan 2025 10:39:43 GMT
Content-Type: text/html
Content-Length: 274
Connection: keep-alive


Verifying purge for http://localhost:8083/one.png (expecting MISS)
HTTP/1.1 200 OK
Server: nginx/1.24.0
Date: Thu, 16 Jan 2025 10:39:43 GMT
Content-Type: image/jpeg
Content-Length: 47579
Connection: keep-alive
Last-Modified: Tue, 14 Jan 2025 19:07:07 GMT
ETag: "6786b5db-b9db"
X-Cache-Status: MISS
Accept-Ranges: bytes


Verifying cache status for http://localhost:8083/two.png (expecting HIT)
HTTP/1.1 200 OK
Server: nginx/1.24.0
Date: Thu, 16 Jan 2025 10:39:43 GMT
Content-Type: image/jpeg
Content-Length: 138460
Connection: keep-alive
Last-Modified: Tue, 14 Jan 2025 19:08:13 GMT
ETag: "6786b61d-21cdc"
X-Cache-Status: HIT
Accept-Ranges: bytes
```
