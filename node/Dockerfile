# Stage 1: Build C++ Node
FROM alpine:latest AS builder
RUN apk add --no-cache g++ cmake make

WORKDIR /app
COPY . .

RUN mkdir build && cd build && cmake .. && make

# Stage 2: Minimal Runtime
FROM alpine:latest
RUN apk add --no-cache libstdc++

WORKDIR /app
COPY --from=builder /app/build/HydraNode .
RUN mkdir data

EXPOSE 8333
CMD ["./HydraNode"]
