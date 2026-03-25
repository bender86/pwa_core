// wwwroot/service-worker.js

self.addEventListener('install', () => self.skipWaiting());
self.addEventListener('activate', () => self.clients.claim());

// Receive push notification from server
self.addEventListener('push', event => {
    if (!event.data) return;

    const data = event.data.json();

    event.waitUntil(
        self.registration.showNotification(data.title, {
            body:  data.body,
            icon:  data.icon  || '/icon-192.png',
            badge: data.badge || '/badge-72.png',
            data:  { url: data.url || '/worldbet/predictions' }
        })
    );
});

// Handle click on notification
self.addEventListener('notificationclick', event => {
    event.notification.close();

    event.waitUntil(
        clients.matchAll({ type: 'window', includeUncontrolled: true }).then(clientList => {
            const url = event.notification.data?.url || '/';

            // Focus existing window if already open
            for (const client of clientList) {
                if (client.url.includes(self.location.origin) && 'focus' in client) {
                    client.navigate(url);
                    return client.focus();
                }
            }

            // Otherwise open a new window
            return clients.openWindow(url);
        })
    );
});