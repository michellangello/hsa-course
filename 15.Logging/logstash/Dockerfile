# Базовий образ від Elastic
FROM docker.elastic.co/logstash/logstash:7.17.9

USER root

RUN /usr/share/logstash/bin/logstash-plugin install logstash-output-gelf

ENV LS_JAVA_OPTS="-Xms512m -Xmx512m"

COPY pipeline/ /usr/share/logstash/pipeline/

USER logstash

CMD ["/usr/local/bin/docker-entrypoint", "--path.config=/usr/share/logstash/pipeline/mysql.conf", "--config.reload.automatic"]