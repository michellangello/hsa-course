#!/bin/bash

# Define URLs
URL1="http://localhost:8083/one.png"
URL2="http://localhost:8083/two.png"
PURGE_URL1="http://localhost:8083/purge/one.png"

# Function to request a URL twice and display cache status
request_twice() {
  local url=$1
  echo "Requesting $url (expecting MISS)"
  curl -I $url
  echo ""

  echo "Requesting $url (expecting HIT)"
  curl -I $url
  echo ""
}

# Function to purge a URL
purge_url() {
  local purge_url=$1
  echo "Purging cache for $purge_url"
  curl -I -H "Cache-Control: no-cache" $purge_url
  echo ""
}

# Step 1: Request one.png twice
request_twice $URL1

# Step 2: Request two.png twice
request_twice $URL2

# Step 3: Purge one.png
purge_url $PURGE_URL1

# Step 4: Verify purge for one.png
echo "Verifying purge for $URL1 (expecting MISS)"
curl -I $URL1
echo ""

# Step 5: Verify two.png remains cached
echo "Verifying cache status for $URL2 (expecting HIT)"
curl -I $URL2
echo ""