#!/bin/sh

_VERSION=`cat .sbver`

echo "Geting files ..."
git add .
echo "Commiting all files ...: Version $_VERSION"
git commit -m "$_VERSION"
echo "Pushing repository on server GITHUB ..."
git push
echo "All done ..."
echo "..."