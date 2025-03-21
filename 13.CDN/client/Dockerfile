FROM debian:bookworm-slim

RUN apt-get update && \
    apt-get install -y dnsutils siege curl iputils-ping && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

CMD ["tail", "-f", "/dev/null"]