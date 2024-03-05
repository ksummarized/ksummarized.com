FROM quay.io/keycloak/keycloak:23.0.6 AS builder

WORKDIR /opt/keycloak
RUN /opt/keycloak/bin/kc.sh build

# Used to install dependencies for keycloak: curl (for running docker healthcheck)
FROM registry.access.redhat.com/ubi9/ubi:9.3-1552 AS ubi-micro-build
RUN mkdir -p /mnt/rootfs
RUN dnf install --installroot=/mnt/rootfs curl-7.76.1 --releasever=9 --setopt=install_weak_deps=false --nodocs -y && \
    dnf --installroot /mnt/rootfs clean all && \
    rpm --root /mnt/rootfs -e --nodeps setup

FROM quay.io/keycloak/keycloak:23.0.6
COPY --from=builder /opt/keycloak/ /opt/keycloak/
COPY --from=ubi-micro-build /mnt/rootfs /
WORKDIR /opt/keycloak

ENTRYPOINT ["/opt/keycloak/bin/kc.sh"]
