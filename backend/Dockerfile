FROM openjdk:8-jdk-alpine

VOLUME /tmp

ADD backend-snapshot.jar app.jar

EXPOSE 8051:8051

ENV JAVA_OPTS=""
ENTRYPOINT ["java","-Djava.security.egd=file:/dev/./urandom","-jar","/app.jar"]
