# Running from Docker

1. Build docker image from docker file
2. Save docker image as tar file with `docker save -o veribot.tar veribot:latest`
3. Copy image to host with scp `scp ./veribot.tar user@host:VeriBotImage`
4. On host, load image into docker `docker load -i veribot.tar`
5. Run in background on docker host `docker run -d veribot:latest`