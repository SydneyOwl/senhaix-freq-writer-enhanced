FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /source
ENV TZ=Asia/Shanghai
ARG DEBIAN_FRONTEND=noninteractive
COPY . .
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone && \
    sed -i 's/deb.debian.org/mirrors.ustc.edu.cn/g' /etc/apt/sources.list && \
    apt-get update -y && \
    apt-get install --yes --no-install-recommends \
    wget \
    gcc && \
    rm -rf /var/lib/apt/lists/* && \
    wget -O ./amsat-all-frequencies.json https://cdn.jsdelivr.net/gh/palewire/amateur-satellite-database/data/amsat-all-frequencies.json && \
    sed -i 's/@COMMIT_HASH@/DockerVersion/g' Properties/VERSION.cs && \
    sed -i 's/@TAG_NAME@/DockerVersion/g' Properties/VERSION.cs && \
    sed -i "s/@BUILD_TIME@/$(date)/g" Properties/VERSION.cs && \
    chmod +x entrypoint.sh
ENTRYPOINT ["/source/entrypoint.sh"]
