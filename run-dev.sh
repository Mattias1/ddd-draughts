#!/bin/bash

# --- Parse parameters ---
if [[ $ACTION::1 == "-" ]] ;
then
    FLAG=$1
    ACTION=$2
else
    ACTION=$1
    FLAG=$2
fi

# --- The actions we can execute ---
function start {
    echo "Starting draughts-dev dependencies."
    docker-compose -p draughts-dev -f docker/docker-compose-develop.yaml up -d
}

function stop {
    echo "Stopping draughts-dev dependencies."
    docker-compose -p draughts-dev -f docker/docker-compose-develop.yaml stop
}

function remove {
    echo "Stopping and removing draughts-dev dependencies."
    docker-compose -p draughts-dev -f docker/docker-compose-develop.yaml down
}

function reset {
    if [[ $FLAG == "-f" || $FLAG == "--force" ]] ;
    then
        echo "Removing draughts-dev dependencies, including the database content."
        docker-compose -p draughts-dev -f docker/docker-compose-develop.yaml down
        docker volume rm draughts-dev_draughts-db-data
    else
        echo "Warning, resetting will remove the database content. If you are sure, use the --force flag"
    fi
}

function help {
    echo "usage: run-dev.sh [start|stop|remove|restart|reset|hardrestart] [--force]"
}

# --- Handle parameters ---
case ${ACTION} in
    start|up|run|on)
        start;;
    stop|close|off)
        stop;;
    remove|rm|down|shutdown|kill)
        remove;;
    reset|erase|purge)
        reset;;
    restart|rerun)
        stop;start;;
    hardrestart)
        reset;start;;
    *)
        help;;
esac
