#!/bin/bash
SCRIPT=$(realpath "$0")
BASEDIR=$(dirname "$SCRIPT")

# add shims folder to PATH
ADD_PATH_LINE="PATH=\$PATH:$BASEDIR/shims/"
grep -qxF $ADD_PATH_LINE ~/.bashrc || echo $ADD_PATH_LINE >> ~/.bashrc

. ~/.bashrc
dev --help

echo 'Run ". ~/.bashrc" to apply to current session or restart your shell.'