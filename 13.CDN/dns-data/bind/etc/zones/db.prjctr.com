$TTL    604800
@       IN      SOA     ns.prjctr.com. admin.prjctr.com. (
                              2025032102
                         604800
                          86400
                        2419200
                         604800 )

        IN      NS      ns.prjctr.com.

; DNS сервер
ns      IN      A       172.20.0.10

; Round-robin для cdn
cdn     IN      A       172.20.0.11
cdn     IN      A       172.20.0.12

; Гео-сабдомени
us      IN      A       172.20.0.11     ; lb1
eu      IN      A       172.20.0.12     ; lb2