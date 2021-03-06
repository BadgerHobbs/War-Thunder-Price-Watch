# War Thunder Price Watch

This application is a simple scraper and webpage for the War Thunder Store, generating (and updating) a database of items, prices and ratings for everything on the store, with a front-end to view items on sale and their price history.

## Building & Running

```
docker build -t war-thunder-store-scraper:latest -f "War Thunder Store Scraper/Dockerfile" .

docker run -d \
    --name war-thunder-store-scraper \
    -v /path/to/data:/data \
    -e DATABASE_DIRECTORY="/data/" \
    -e REFRESH_INTERVAL="1440 " \
    war-thunder-store-scraper:latest
```

## Todo

- [x] Ability to run as a docker container service
- [ ] Additional front-end website or API
- [ ] Archive of additional media for store items
- [ ] Archive of original webpages for store items
- [ ] Storage of prices in multiple currencies
- [ ] Support for other Gaijin games other than War Thunder
- [ ] Possibly import old prices from Archive.org/Wayback Machine
