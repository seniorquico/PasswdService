#!/usr/bin/env bash
set -eufo pipefail

########## Required Environment Variables ##########

if [ -z ${IS_PULL_REQUEST+x} ]
then
  echo 'IS_PULL_REQUEST is undefined'
  exit 1
fi

if [ -z ${SHIPPABLE_BUILD_DIR+x} ]
then
  echo 'SHIPPABLE_BUILD_DIR is undefined'
  exit 1
fi

########## Optional Environment Variables ##########

if [ -z ${DOCKER_COMPOSE_VERSION+x} ]
then
  export DOCKER_COMPOSE_VERSION=1.24.0
fi

########## Script ##########

# Initialize Docker Compose
echo 'Installing Docker Compose...'
echo "curl -sSL \"https://github.com/docker/compose/releases/download/${DOCKER_COMPOSE_VERSION}/docker-compose-$( uname -s )-$( uname -m )\" -o /usr/local/bin/docker-compose"
curl -sSL "https://github.com/docker/compose/releases/download/${DOCKER_COMPOSE_VERSION}/docker-compose-$( uname -s )-$( uname -m )" -o /usr/local/bin/docker-compose
echo 'chmod +x /usr/local/bin/docker-compose'
chmod +x /usr/local/bin/docker-compose
echo 'docker-compose --version'
docker-compose --version
echo 'Installed Docker Compose'

# Install xsltproc
echo 'Installing xsltproc...'
echo 'apt-get --assume-yes install xsltproc'
apt-get --assume-yes install xsltproc
echo 'Installed xsltproc'

# Copy source code
echo 'Copying source code to Docker work volume...'
echo "cd \"${SHIPPABLE_BUILD_DIR}\""
cd "${SHIPPABLE_BUILD_DIR}"
set +f
echo 'rm -rf /work/{*,.*} || true'
rm -rf /work/{*,.*} || true
set -f
echo 'cp -R . /work'
cp -R . /work
echo 'Copied source code to Docker work volume'

# Build project
echo 'Building project...'
echo 'cd /work'
cd /work
echo 'docker-compose -f ./docker-compose.ci.build.yml -f ./docker-compose.ci.build.shippable.yml up'
docker-compose -f ./docker-compose.ci.build.yml -f ./docker-compose.ci.build.shippable.yml up
echo 'docker-compose -f ./docker-compose.ci.build.yml -f ./docker-compose.ci.build.shippable.yml down'
docker-compose -f ./docker-compose.ci.build.yml -f ./docker-compose.ci.build.shippable.yml down
echo 'Built project'

# Test project
echo 'Testing project...'
echo 'cd /work'
cd /work
echo 'docker-compose -f ./docker-compose.ci.test.yml -f ./docker-compose.ci.test.shippable.yml up'
docker-compose -f ./docker-compose.ci.test.yml -f ./docker-compose.ci.test.shippable.yml up
echo 'docker-compose -f ./docker-compose.ci.test.yml -f ./docker-compose.ci.test.shippable.yml down'
docker-compose -f ./docker-compose.ci.test.yml -f ./docker-compose.ci.test.shippable.yml down
echo "mkdir -p \"${SHIPPABLE_BUILD_DIR}/shippable/testresults\""
mkdir -p "${SHIPPABLE_BUILD_DIR}/shippable/testresults"
echo "xsltproc -o \"${SHIPPABLE_BUILD_DIR}/shippable/testresults/PasswdService.Tests.xml\" \"${SHIPPABLE_BUILD_DIR}/shippable/trx-to-junit.xslt\" '/work/testresults/PasswdService.Tests.xml'"
xsltproc -o "${SHIPPABLE_BUILD_DIR}/shippable/testresults/PasswdService.Tests.xml" "${SHIPPABLE_BUILD_DIR}/shippable/trx-to-junit.xslt" '/work/testresults/PasswdService.Tests.xml'
echo "mkdir -p \"${SHIPPABLE_BUILD_DIR}/shippable/codecoverage\""
mkdir -p "${SHIPPABLE_BUILD_DIR}/shippable/codecoverage"
echo "cp '/work/coverage/PasswdService.Tests.xml' \"${SHIPPABLE_BUILD_DIR}/shippable/codecoverage/coverage.xml\""
cp '/work/coverage/PasswdService.Tests.xml' "${SHIPPABLE_BUILD_DIR}/shippable/codecoverage/coverage.xml"
echo 'Tested project'

if [ "$IS_PULL_REQUEST" != true ]
then
  # Build Docker image
  echo 'Building Docker image...'
  echo 'docker-compose -f ./docker-compose.yml build'
  docker-compose -f ./docker-compose.yml build
  echo 'Built Docker image'

  # Tag Docker image
  echo 'Tagging Docker image...'
  echo "docker tag \"seniorquico/passwd-service\" \"seniorquico/passwd-service:${COMMIT}\""
  docker tag "seniorquico/passwd-service" "seniorquico/passwd-service:${COMMIT}"
  echo 'docker images'
  docker images
  echo 'Tagged Docker images'

  # Push Docker images
  echo 'Pushing Docker images...'
  echo "docker push \"seniorquico/passwd-service:${COMMIT}\""
  docker push "seniorquico/passwd-service:${COMMIT}"
  echo 'Pushed Docker images'
fi
