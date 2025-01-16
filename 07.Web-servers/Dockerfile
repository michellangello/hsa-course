FROM debian:bullseye

# Install build dependencies
RUN apt-get update && apt-get install -y \
    gcc \
    g++ \
    make \
    curl \
    libpcre3-dev \
    libssl-dev \
    zlib1g-dev \
    git \
    wget \
    && rm -rf /var/lib/apt/lists/*

# Define Nginx version
ENV NGINX_VERSION=1.24.0

# Download Nginx source and ngx_cache_purge module
RUN cd /tmp && \
    wget http://nginx.org/download/nginx-${NGINX_VERSION}.tar.gz && \
    tar -zxvf nginx-${NGINX_VERSION}.tar.gz && \
    git clone https://github.com/FRiCKLE/ngx_cache_purge.git

# Build Nginx with ngx_cache_purge
RUN cd /tmp/nginx-${NGINX_VERSION} && \
    ./configure \
        --prefix=/etc/nginx \
        --sbin-path=/usr/sbin/nginx \
        --modules-path=/usr/lib/nginx/modules \
        --conf-path=/etc/nginx/nginx.conf \
        --error-log-path=/var/log/nginx/error.log \
        --http-log-path=/var/log/nginx/access.log \
        --pid-path=/var/run/nginx.pid \
        --lock-path=/var/run/nginx.lock \
        --http-client-body-temp-path=/var/cache/nginx/client_temp \
        --http-proxy-temp-path=/var/cache/nginx/proxy_temp \
        --http-fastcgi-temp-path=/var/cache/nginx/fastcgi_temp \
        --http-uwsgi-temp-path=/var/cache/nginx/uwsgi_temp \
        --http-scgi-temp-path=/var/cache/nginx/scgi_temp \
        --with-http_ssl_module \
        --with-http_stub_status_module \
        --with-http_realip_module \
        --with-http_gzip_static_module \
        --with-http_v2_module \
        --with-threads \
        --with-file-aio \
        --add-module=/tmp/ngx_cache_purge && \
    make && \
    make install

# Create required directories for caching
RUN mkdir -p /var/cache/nginx && chmod 777 /var/cache/nginx

# Clean up
RUN rm -rf /tmp/nginx-${NGINX_VERSION} /tmp/ngx_cache_purge

# Expose port 80
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]