const cacheName = "offline";
const host = location.host;

const urlsToCache = [
    "./",
    "./manifest.json",
    "./favicon.ico",
    "./Build/Build.data",
    "./Build/Build.framework.js",
    "./Build/Build.loader.js",
    "./Build/Build.wasm",
];
self.addEventListener("install", e =>
    e.waitUntil(
        caches.open(cacheName).then(cache =>
            cache.addAll(urlsToCache)
        )
    )
);

self.addEventListener("fetch", (e) =>
{
    const reqHost = new URL(e.request.url).host;
    if (reqHost != host) return;

    e.respondWith((async () =>
    {
        const cache = await caches.open(cacheName);
        try
        {
            const fetchedResponse = await fetch(e.request.url);
            cache.put(e.request, fetchedResponse.clone());
            return fetchedResponse;
        }
        catch
        {
            return await cache.match(e.request.url);
        }
    })());
});
