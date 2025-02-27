# Solving Peak Loadings for Goal.com

## Page Types
- **Homepage**
- **Match Live Score Pages**
- **Article Pages**
- **League & Team & Player Profiles**

## Possible Sources of Peak Loadings
- **Live Matches & Major Events**: Sudden influx of users during major football matches and tournaments.
- **Breaking News**: High user demand when major football news is published.
- **Search Engine Crawlers**: Increased bot activity indexing pages.
- **DDoS Attacks**: Malicious traffic overwhelming servers.
- **Push Notifications**: Sudden spikes in traffic when users receive and act on notifications.

## Solutions

- **CDN Caching**: Cache static content and pre-rendered HTML at the edge.
- **Auto-scaling Infrastructure**: Prepare for matches and tournaments based on the schedule with dynamically scaling application servers
- **Asynchronous Processing**: Process analytics, insights asynchronously to avoid blocking. Try to avoid save data to DB directly where it is possible.
- **Popular resources caching**: Cache news, feeds on the homepage to reduce database load.
- **Lazy Load Videos**: Load videos only when they are about to be played.
